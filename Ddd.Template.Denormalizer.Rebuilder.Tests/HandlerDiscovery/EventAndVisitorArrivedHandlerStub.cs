using System;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Events.Visitor;
using NServiceBus;

namespace Ddd.Template.Denormalizer.Rebuilder.Tests.HandlerDiscovery
{
	internal sealed class EventAndVisitorArrivedHandlerStub : IHandleMessages<Event>, IHandleMessages<VisitorArrived>
	{
		public void Handle(Event message)
		{
			Console.WriteLine("Event was handled.");
		}

		public void Handle(VisitorArrived message)
		{
			Console.WriteLine("VisitorArrived was handled.");
		}
	}
}
