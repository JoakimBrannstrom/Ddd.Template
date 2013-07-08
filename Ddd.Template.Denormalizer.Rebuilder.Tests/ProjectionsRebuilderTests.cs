using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Denormalizer.Projections;
using Ddd.Template.Denormalizer.Raven;
using Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Raven.Client.Embedded;

namespace Ddd.Template.Denormalizer.Rebuilder.Tests
{
	[TestClass]
	public sealed class ProjectionsRebuilderTests
	{
		[TestMethod]
		public void GivenVisitorEventsExistInStream_WhenRebuildingDenormalizedView_ThenVisitorProjectionsSholdExistInStorage()
		{
			// Arrange
			using (var store = new EmbeddableDocumentStore { RunInMemory = true })
			{
				store.Initialize();

				// const int nrOfVisitors = 1000;
				const int nrOfVisitors = 10;

				var eventStorage = new Mock<IEventStorage>();
				eventStorage
					.Setup(e => e.GetAll())
					.Returns(() => GetFakeHistory(nrOfVisitors));

				var visitorDenormalizer = new VisitorDenormalizer(store);
				var handlerInvoker = new HandlerInvoker(new object[] { visitorDenormalizer });

				var projectionsRebuilder = new ProjectionsRebuilder(eventStorage.Object, handlerInvoker);

				// Act
				var timer = new Stopwatch();
				timer.Start();
				projectionsRebuilder.Rebuild();
				timer.Stop();

				// Assert
				int visitorCount;
				using (var session = store.OpenSession())
				{
					visitorCount = session
									.Query<Visitor>()
									.Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
									.Count();
				}

				Console.WriteLine("Denormalized view was rebuilt, nr of visitors: '" + visitorCount + "', time: " + timer.Elapsed);
				Assert.AreEqual(nrOfVisitors, visitorCount);
			}
		}

		private static IEnumerable<Event> GetFakeHistory(int nrOfVisitors)
		{
			var history = new List<Event>();

			for (var i = 0; i < nrOfVisitors; i++)
			{
				history.AddRange(GetVisitorEvents(Guid.NewGuid()));
			}

			return history.OrderBy(e => e.Version);
		}

		private static IEnumerable<Event> GetVisitorEvents(Guid aggregateId)
		{
			return new [] { new VisitorArrived { AggregateId = aggregateId } };
			/*
			return new VisitorEventGenerator(aggregateId)
						.VisitorArrived(url + counter)
						.VisitorLoggedIn("test@email.com")
						.GetEvents();
			*/
		}
	}
}
