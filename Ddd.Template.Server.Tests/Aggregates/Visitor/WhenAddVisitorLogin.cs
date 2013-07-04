using System;
using System.Collections.Generic;
using Ddd.Template.Contracts.Commands.Visitor;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Ddd.Template.Server.Tests.Aggregates.Visitor
{
	public abstract class WhenAddVisitorLogin : CommandTestFixture<Domain.Aggregates.Visitor, AddVisitorLogin, VisitorCommandHandler>
	{
		protected abstract Guid UserId { get; }

		protected override IEnumerable<Event> Given()
		{
			return new[] { new VisitorArrived { AggregateId = AggregateId } };
		}

		protected override AddVisitorLogin When()
		{
			return new AddVisitorLogin
			{
				AggregateId = AggregateId,
				CommandId = Guid.NewGuid(),
				OriginalVersion = 1,

				UserId = UserId
			};
		}

		[TestClass]
		public class WhenAddVisitorLogin_WithProperUserId : WhenAddVisitorLogin
		{
			protected override Guid UserId { get { return Guid.NewGuid(); } }

			[TestMethod]
			public void OneEventShouldBePublishedOnTheBus()
			{
				VerifyThatEventsWasPublishedOnTheBus(Times.Once());
			}

			[TestMethod]
			public void ThePublishedEventShouldBeVisitorLoggedIn()
			{
				VerifyThatEventPublishedOnTheBusIsOfCorrectType<VisitorLoggedIn>();
			}

			[TestMethod]
			public void ThePublishedEventShouldHaveCurrentTimestamp()
			{
				VerifyThatEventsPublishedOnTheBusHaveCurrentTimestamp();
			}
		}
	}
}
