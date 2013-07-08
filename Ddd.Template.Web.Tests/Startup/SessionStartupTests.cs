using System;
using Ddd.Template.Contracts.Commands.Visitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Ddd.Template.Web.Tests.Startup
{
	[TestClass]
	public sealed class SessionStartupTests : ApplicationTestsBase
	{
		[TestMethod]
		public void GivenNewVisitor_WhenSessionIsStarted_ThenAddVisitorCommandShouldBeSent()
		{
			// Arrange
			FakeBus
				.Setup(b => b.Send(It.IsAny<AddVisitor>()))
				.Verifiable();

			// Act
			Session_Start(null, null);

			// Assert
			FakeBus.Verify();
		}

		[TestMethod]
		public void GivenNewVisitor_WhenSessionIsStarted_ThenVisitorIdShouldBeStoredInSession()
		{
			// Arrange
			FakeSession
				.Setup(s => s.Add("VisitorId", It.IsAny<Guid>()))
				.Verifiable();

			// Act
			Session_Start(null, null);

			// Assert
			FakeSession.Verify();
		}
	}
}
