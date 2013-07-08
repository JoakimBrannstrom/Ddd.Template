using System;
using System.Collections.Generic;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Commands.Visitor;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Ddd.Template.Server.Tests.Aggregates.Visitor
{
	public class WhenVisitorIsAdded : CommandTestFixture<Domain.Aggregates.Visitor, AddVisitor, VisitorCommandHandler>
	{
		protected override AddVisitor When()
		{
			return new AddVisitor
			{
				AggregateId = AggregateId,
				CommandId = Guid.NewGuid(),
				Created = DateTime.Now,
				Platform = "Platform",
				UserAgent = "UserAgent",
				UserHostAddress = "UserHostAddress",
				UserHostName = "UserHostName",
				UserLanguages = new List<string> { "UserLanguages" },
				OriginalVersion = 0
			};
		}

		[TestClass]
		public sealed class WhenVisitorArrived_WithNoHistory : WhenVisitorIsAdded
		{
			[TestMethod]
			public void OneEventShouldBePublishedOnTheBus()
			{
				VerifyThatEventsWasPublishedOnTheBus(Times.Once());
			}

			[TestMethod]
			public void ThePublishedEventShouldBeVisitorArrived()
			{
				VerifyThatEventPublishedOnTheBusIsOfCorrectType<VisitorArrived>();
			}

			[TestMethod]
			public void ThePublishedEventShouldHaveCurrentTimestamp()
			{
				VerifyThatEventsPublishedOnTheBusHaveCurrentTimestamp();
			}
		}

		[TestClass]
		public sealed class WhenVisitorArrived_CalledTwice : WhenVisitorIsAdded
		{
			protected override Type GetExpectedExceptionType()
			{
				return typeof(ConcurrencyException);
			}

			protected override IEnumerable<Event> Given()
			{
				return new[] { new VisitorArrived { AggregateId = AggregateId } };
			}

			[TestMethod]
			public void ConcurrencyExceptionShouldBeThrown()
			{
				VerifyLastException<ConcurrencyException>();
			}
			[TestMethod]
			public void NoEventsShouldBePublishedOnTheBus()
			{
				VerifyThatNoEventsWasPublishedOnTheBus();
			}
		}
	}
}
