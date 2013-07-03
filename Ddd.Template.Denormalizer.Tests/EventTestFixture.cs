using System;
using System.Collections.Generic;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Scaffolding;
using Ddd.Template.Denormalizer.Tests.Scaffolding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;
using Raven.Client;

namespace Ddd.Template.Denormalizer.Tests
{
	/// <summary>
	/// Heavily inspired by https://gist.github.com/MarkNijhof/Fohjin
	/// Read more at: http://cre8ivethought.com/blog/2009/12/22/specifications/
	/// </summary>
	/// <typeparam name="TEvent">The event that should be used for When (GWT)</typeparam>
	/// <typeparam name="TEventHandler">The test is only for one event-handler</typeparam>
	[TestClass]
	public abstract class EventTestFixture<TEvent, TEventHandler> : IDisposable
		where TEvent : Event
		where TEventHandler : IHandleMessages<TEvent>
	{
		private DependencyConfigurator<TEvent, TEventHandler> _dependencyConfigurator;

		protected readonly Guid AggregateId = Guid.NewGuid();

		public void Dispose()
		{
			_dependencyConfigurator.Dispose();
		}

		protected virtual IEnumerable<Event> Given()
		{
			return new List<Event>();
		}

		protected abstract TEvent When();

		[TestInitialize]
		public void Setup()
		{
			var eventHandler = GetEventHandler();

			ReplayHistoricalEvents(eventHandler);

			RunTestScenario(eventHandler);
		}

		private IHandleMessages<TEvent> GetEventHandler()
		{
			_dependencyConfigurator = new DependencyConfigurator<TEvent, TEventHandler>();

			return _dependencyConfigurator.BuildEventHandler();
		}

		private void ReplayHistoricalEvents(IHandleMessages<TEvent> eventHandler)
		{
			var dynamicEventHandler = eventHandler.AsDynamic();
			var events = Given();

			foreach (var @event in events)
			{
				dynamicEventHandler.Handle(@event);
			}
		}

		private void RunTestScenario(IHandleMessages<TEvent> eventHandler)
		{
			eventHandler.Handle(When());
		}

		protected void VerifyThatProjectionExistsInDomainView<T>(Guid id)
		{
			var result = GetProjectionFromDomainView<T>(id);

			Assert.IsNotNull(result);
		}

		protected void VerifyThatProjectionDoesNotExistInDomainView<T>(Guid id)
		{
			var result = GetProjectionFromDomainView<T>(id);

			Assert.IsNull(result);
		}

		protected T GetProjectionFromDomainView<T>(Guid id)
		{
			var store = _dependencyConfigurator
							.GetDependency<IDocumentStore>();

			using (var session = store.OpenSession())
			{
				return session.Load<T>(id);
			}
		}
	}
}
