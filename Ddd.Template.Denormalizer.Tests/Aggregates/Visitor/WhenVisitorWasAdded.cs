using System;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Denormalizer.Raven;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Denormalizer.Tests.Aggregates.Visitor
{
	[TestClass]
	public class WhenVisitorWasAdded : EventTestFixture<VisitorArrived, VisitorDenormalizer>
	{
		private const string Platform = "Unit test";
		readonly DateTime _timestamp = DateTime.UtcNow;

		protected override VisitorArrived When()
		{
			return new VisitorArrived
			{
				AggregateId = AggregateId,
				Created = DateTime.UtcNow.AddMinutes(-1),
				Platform = Platform,
				UtcTimestamp = _timestamp,
				Version = 0,
			};
		}

		[TestMethod]
		public void AVisitorProjectionSholdBeInStorage()
		{
			VerifyThatProjectionExistsInDomainView<Projections.Visitor>(AggregateId);
		}

		[TestMethod]
		public void TheVisitorProjectionSholdHaveCorrectPlatform()
		{
			var visitor = GetProjectionFromDomainView<Projections.Visitor>(AggregateId);
			Assert.AreEqual(Platform, visitor.Platform);
		}

		[TestMethod]
		public void TheVisitorProjectionSholdHaveCorrectModifiedDate()
		{
			var visitor = GetProjectionFromDomainView<Projections.Visitor>(AggregateId);
			Assert.AreEqual(_timestamp, visitor.UtcModified);
		}
	}
}
