using System;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Denormalizer.Raven;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Denormalizer.Tests.Aggregates.Visitor
{
	[TestClass]
	public class WhenVisitorLoggedIn : EventTestFixture<VisitorLoggedIn, VisitorDenormalizer>
	{
		readonly Guid _userId = Guid.NewGuid();
		readonly DateTime _timestamp = DateTime.UtcNow;

		protected override VisitorLoggedIn When()
		{
			return new VisitorLoggedIn
			{
				AggregateId = AggregateId,
				UserId = _userId,
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
		public void TheVisitorProjectionSholdBeUpdatedWithUserId()
		{
			var visitor = GetProjectionFromDomainView<Projections.Visitor>(AggregateId);
			Assert.AreEqual(_userId, visitor.UserId);
		}

		[TestMethod]
		public void TheVisitorProjectionSholdHaveCorrectModifiedDate()
		{
			var visitor = GetProjectionFromDomainView<Projections.Visitor>(AggregateId);
			Assert.AreEqual(_timestamp, visitor.UtcModified);
		}
	}
}
