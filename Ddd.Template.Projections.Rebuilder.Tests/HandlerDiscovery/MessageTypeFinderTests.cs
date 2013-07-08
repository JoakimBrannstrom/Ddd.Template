using System.Linq;
using Ddd.Template.Projections.Rebuilder.HandlerDiscovery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Projections.Rebuilder.Tests.HandlerDiscovery
{
	[TestClass]
	public sealed class MessageTypeFinderTests
	{
		[TestMethod]
		public void GivenHandlerThatImplementsSeveralMatchingHandleMethodsForEventExists_WhenHandlerInvokerRuns_ThenAllHandlerMethodsShouldBeInvoked()
		{
			// Arrange

			// Act
			var methods = MessageTypeFinder.GetHandledMessageTypes(typeof(EventAndVisitorArrivedHandlerStub)).ToArray();

			// Assert
			Assert.IsNotNull(methods);
			Assert.AreEqual(2, methods.Count(), "Expected method info for 'Event' and 'VisitorArrived'!");
		}
	}
}
