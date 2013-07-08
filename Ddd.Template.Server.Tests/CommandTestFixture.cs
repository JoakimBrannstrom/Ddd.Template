using System;
using System.Collections.Generic;
using System.Linq;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Commands;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Server.CommitDispatchers;
using Ddd.Template.Server.Tests.Scaffolding;
using EventStore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using NServiceBus;
using Raven.Client;

namespace Ddd.Template.Server.Tests
{
	/// <summary>
	/// Heavily inspired by https://gist.github.com/MarkNijhof/Fohjin
	/// Read more at: http://cre8ivethought.com/blog/2009/12/22/specifications/
	/// </summary>
	/// <typeparam name="TAggregateRoot">The aggregate root that your test will cover</typeparam>
	/// <typeparam name="TCommand">The command that should be used for When (GWT)</typeparam>
	/// <typeparam name="TCommandHandler">The test is only for one command-handler</typeparam>
	[TestClass]
	public abstract class CommandTestFixture<TAggregateRoot, TCommand, TCommandHandler> : IDisposable
		where TAggregateRoot : AggregateRoot, new()
		where TCommand : Command
		where TCommandHandler : IHandleMessages<TCommand>
	{
		private DependencyConfigurator<TAggregateRoot, TCommand, TCommandHandler> _dependencyConfigurator;

		private Exception _lastException;
		private List<Event> _publishedMessages;

		protected Guid AggregateId = Guid.NewGuid();

		public void Dispose()
		{
			_dependencyConfigurator.Dispose();
		}

		protected virtual IEnumerable<Event> Given()
		{
			return new List<Event>();
		}

		protected abstract TCommand When();

		protected virtual Type GetExpectedExceptionType()
		{
			return null;
		}

		[TestInitialize]
		public void Setup()
		{
			var commandHandler = GetCommandHandler();

			SaveHistoryInEventStore(Given());

			PrepareBusForTest();

			RunTestScenario(commandHandler);
		}

		private IHandleMessages<TCommand> GetCommandHandler()
		{
			_dependencyConfigurator = new DependencyConfigurator<TAggregateRoot, TCommand, TCommandHandler>();
			_dependencyConfigurator.SetupDependencies();

			return _dependencyConfigurator.BuildCommandHandler();
		}

		private void SaveHistoryInEventStore(IEnumerable<Event> history)
		{
			// Use fake-bus when storing history, we don't want the history to be published on the bus
			PrepareFakeBusForStoringHistory();

			AddHistoryToEventStorage(history.ToArray());
		}

		private void PrepareFakeBusForStoringHistory()
		{
			var publisher = _dependencyConfigurator.GetMockedDependency<NServiceBusPublisher>();

			publisher
					.Protected()
					.Setup<IBus>("Bus")
					.Returns(new Mock<IBus>().Object);
		}

		private void AddHistoryToEventStorage(Event[] history)
		{
			var eventStore = _dependencyConfigurator.GetDependency<IStoreEvents>();

			var aggregateIds = history.Select(h => h.AggregateId).Distinct();

			foreach (var aggregateId in aggregateIds)
			{
				var id = aggregateId;
				var aggregateHistory = history.Where(h => h.AggregateId == id).ToArray();

				AddAggregateHistoryToEventStorage(aggregateHistory, eventStore);
			}
		}

		private static void AddAggregateHistoryToEventStorage(Event[] history, IStoreEvents eventStore)
		{
			var aggregateId = history.First().AggregateId;

			using (var stream = eventStore.OpenStream(aggregateId, 0, int.MaxValue))
			{
				foreach (var @event in history)
					stream.Add(new EventMessage { Body = @event });

				stream.CommitChanges(Guid.NewGuid());
			}
		}

		private void PrepareBusForTest()
		{
			var bus = _dependencyConfigurator.GetMockedDependency<IBus>();

			_publishedMessages = new List<Event>();
			bus
				.Setup(x => x.Publish(It.IsAny<Event[]>()))
				.Callback<Event[]>(x => _publishedMessages.AddRange(x));

			_dependencyConfigurator
				.GetMockedDependency<NServiceBusPublisher>()
				.Protected()
				.Setup<IBus>("Bus")
				.Returns(bus.Object);
		}

		private void RunTestScenario(IHandleMessages<TCommand> commandHandler)
		{
			try
			{
				commandHandler.Handle(When());
			}
			catch (Exception exc)
			{
				var expectedExceptionType = GetExpectedExceptionType();
				if (expectedExceptionType == null || expectedExceptionType != exc.GetType())
					throw;

				_lastException = exc;
			}
		}

		protected void VerifyLastException<T>()
		{
			var expectedType = typeof(T);
			Assert.IsNotNull(_lastException, "We should have an " + expectedType.Name + " by now...");
			Assert.AreEqual(expectedType, _lastException.GetType(), "Wrong type of exception! Expected " + expectedType.Name);
		}

		protected void VerifyThatNoEventsWasPublishedOnTheBus()
		{
			_dependencyConfigurator.GetMockedDependency<IBus>().Verify(x => x.Publish(It.IsAny<Event[]>()), Times.Never(), "OMG! We shouldn't have any event on the bus!");
		}

		protected void VerifyThatEventPublishedOnTheBusIsOfCorrectType<T>()
		{
			VerifyThatEventsPublishedOnTheBusIsOfCorrectType(new[] { typeof(T) });
		}

		protected void VerifyThatEventsPublishedOnTheBusIsOfCorrectType(IList<Type> types)
		{
			for (var i = 0; i < types.Count; i++)
			{
				var expectedType = types[i];
				Assert.AreEqual(expectedType, _publishedMessages.Skip(i).First().GetType(), "The generated event should be a " + expectedType.Name + "-event!");
			}
		}

		protected void VerifyThatEventsPublishedOnTheBusHaveCurrentTimestamp()
		{
			foreach (var @event in _publishedMessages)
			{
				var timestamp = @event.UtcTimestamp;
				var span = DateTime.UtcNow - timestamp;
				Assert.IsTrue(span < TimeSpan.FromSeconds(5), "The generated event did not have a current timestamp! It was: " + timestamp);
			}
		}

		protected void VerifyThatEventsWasPublishedOnTheBus(Times nrOfEvents)
		{
			_dependencyConfigurator
				.GetMockedDependency<IBus>()
				.Verify(x => x.Publish(It.IsAny<Event[]>()), nrOfEvents, string.Format("Expected {0} event(s) on the bus!", nrOfEvents));
		}

		protected void VerifyThatProjectionWasStoredInDomainView<T>(Guid id)
		{
			var store = _dependencyConfigurator.GetDependency<IDocumentStore>();
			T result;

			using (var session = store.OpenSession())
			{
				result = session.Load<T>(id);
			}

			Assert.IsNotNull(result);
		}

		protected IEnumerable<Event> GetPublishedMessages()
		{
			return _publishedMessages.AsReadOnly();
		}
	}
}
