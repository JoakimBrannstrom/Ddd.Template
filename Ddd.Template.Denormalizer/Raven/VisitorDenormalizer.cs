using System;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Denormalizer.Projections;
using NServiceBus;
using Raven.Client;

namespace Ddd.Template.Denormalizer.Raven
{
	public sealed class VisitorDenormalizer :	DenormalizerBase,
												IHandleMessages<VisitorArrived>,
												IHandleMessages<VisitorLoggedIn>
	{
		public VisitorDenormalizer(IDocumentStore store)
			: base(store)
		{
		}

		public void Handle(VisitorArrived @event)
		{
			Action<Visitor> action = visitor =>
			{
				visitor.Id = @event.AggregateId;
				visitor.UtcCreated = @event.Created;
				visitor.Platform = @event.Platform;
				visitor.UserAgent = @event.UserAgent;
				visitor.UserHostAddress = @event.UserHostAddress;
				visitor.UserHostName = @event.UserHostName;
				visitor.UserLanguages = @event.UserLanguages;
				visitor.Version = @event.Version;
			};

			HandleEventIfVersionIsSubsequent(@event, action);
		}

		public void Handle(VisitorLoggedIn @event)
		{
			Action<Visitor> action = visitor =>
			{
				visitor.UserId = @event.UserId;
				visitor.Version = @event.Version;
			};

			HandleEventIfVersionIsSubsequent(@event, action);
		}
	}
}
