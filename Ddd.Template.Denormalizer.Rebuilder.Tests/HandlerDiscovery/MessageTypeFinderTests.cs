using System.Linq;
using Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Denormalizer.Rebuilder.Tests.HandlerDiscovery
{
	[TestClass]
	public sealed class MessageTypeFinderTests
	{
		[TestMethod]
		public void GivenHandlerThatImplementsSeveralMatchingHandleMethodsForEventExists_WhenHandlerInvokerRuns_ThenAllHandlerMethodsShouldBeInvoked()
		{
			// Arrange
			var finder = new MessageTypeFinder();

			// Act
			var methods = MessageTypeFinder.GetHandledMessageTypes(typeof(EventAndVisitorArrivedHandlerStub)).ToArray();

			// Assert
			Assert.IsNotNull(methods);
			Assert.AreEqual(2, methods.Count(), "Expected method info for 'Event' and 'VisitorArrived'!");
		}
	}
}
