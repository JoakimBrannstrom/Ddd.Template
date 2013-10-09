using System;
using System.Diagnostics;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Projections.Rebuilder.HandlerDiscovery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NServiceBus;

namespace Ddd.Template.Projections.Rebuilder.Tests.HandlerDiscovery
{
	[TestClass]
	public sealed class HandlerInvokerTests
	{
		[TestMethod]
		public void GivenHandlerForEventExists_WhenHandlerInvokerRuns_ThenHandlerShouldBeInvoked()
		{
			// Arrange
			var fakeEvent = new Mock<Event>();
			var handler = new Mock<IHandleMessages<Event>>();
			handler
				.Setup(h => h.Handle(It.IsAny<Event>()))
				.Verifiable();

			var invoker = new HandlerInvoker(new [] { handler.Object });

			// Act
			invoker.CallHandlers(fakeEvent.Object);

			// Assert
			handler.Verify();
		}

		[TestMethod]
		public void GivenHandlerForEventDoesNotExist_WhenHandlerInvokerRuns_ThenHandlerShouldNotBeInvoked()
		{
			// Arrange
			var fakeEvent = new Mock<Event>();
			var handler = new Mock<IHandleMessages<VisitorArrived>>();
			var invokeCounter = 0;
			handler
				.Setup(h => h.Handle(It.IsAny<VisitorArrived>()))
				.Callback(() => invokeCounter++);

			var invoker = new HandlerInvoker(new [] { handler.Object });

			// Act
			invoker.CallHandlers(fakeEvent.Object);

			// Assert
			Assert.AreEqual(0, invokeCounter, "Handler should not have been invoked!");
		}

		[TestMethod]
		public void GivenHandlerThatImplementsSeveralMatchingHandleMethodsForEventExists_WhenHandlerInvokerRuns_ThenAllHandlerMethodsShouldBeInvoked()
		{
			// Arrange
			var handler = new Mock<IHandleMessages<Event>>();
			var visitorHandler = handler.As<IHandleMessages<VisitorArrived>>();
			handler
				.Setup(h => h.Handle(It.IsAny<Event>()))
				.Verifiable();
			visitorHandler
				.Setup(h => h.Handle(It.IsAny<VisitorArrived>()))
				.Verifiable();

			var invoker = new HandlerInvoker(new [] { visitorHandler.Object });

			var theEvent = new VisitorArrived();

			// Act
			invoker.CallHandlers(theEvent);

			// Assert
			handler.Verify();
			visitorHandler.Verify();
		}

		[TestMethod]
		public void GivenHandlerForEventExists_WhenHandlerInvokerRunsManyEvents_ThenPerformanceShouldBeGood()
		{
			// Arrange
			const int nrOfIterations = 100000;
			var invokeCounter = 0;

			/* this Moq-handler is ~3 times slower than the FakeHandler
			var handler = new Mock<IHandleMessages<Event>>();
			handler
				.Setup(h => h.Handle(It.IsAny<Event>()))
				.Callback(() => invokeCounter++);
			var invoker = new HandlerInvoker(new [] { handler.Object });
			*/

			var handler = new FakeHandler();
			var invoker = new HandlerInvoker(new [] { handler });

			var timer = new Stopwatch();
			timer.Start();

			// Act
			for (var i = 0; i < nrOfIterations; i++)
			{
				var theEvent = new VisitorArrived();
				invoker.CallHandlers(theEvent);
			}

			// Assert
			timer.Stop();
			invokeCounter = handler.Counter;

			Console.WriteLine("{0} nr of calls was completed in {1}", invokeCounter.ToString("N0"), timer.Elapsed);

			Assert.AreEqual(nrOfIterations, invokeCounter);
			Assert.IsTrue(timer.Elapsed < TimeSpan.FromSeconds(1.5),
							string.Format("Replay of {0} events took too long time, it was completed in: {1}",
											nrOfIterations, timer.Elapsed));
		}

		private class FakeHandler : IHandleMessages<Event>
		{
			public int Counter { get; private set; }

			public void Handle(Event @event)
			{
				Counter++;
			}
		}

		[TestMethod]
		public void HowFastIsDotNet()
		{
			// Arrange
			const double nrOfIterations = 40000000;
			double invokeCounter = 0;

			var timer = new Stopwatch();
			timer.Start();

			// Act
			for (var i = 0; i < nrOfIterations; i++)
			{
				invokeCounter = Handler(invokeCounter);
			}

			// Assert
			timer.Stop();

			Console.WriteLine("{0} nr of calls was completed in {1}", invokeCounter.ToString("N0"), timer.Elapsed);
			Assert.IsTrue(timer.Elapsed < TimeSpan.FromSeconds(1.5),
							string.Format("Replay of {0} events took too long time, it was completed in: {1}",
											invokeCounter, timer.Elapsed));
		}

		private double Handler(double previous)
		{
			return ++previous;
		}
	}
}
