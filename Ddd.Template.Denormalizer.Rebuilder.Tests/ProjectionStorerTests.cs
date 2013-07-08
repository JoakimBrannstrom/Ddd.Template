using System;
using System.Linq;
using Ddd.Template.Projections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client.Embedded;

namespace Ddd.Template.Denormalizer.Rebuilder.Tests
{
	[TestClass]
	public sealed class ProjectionStorerTests
	{
		[TestMethod]
		public void GivenProjectionsExistInMemoryStore_WhenProjectionsArePersisted_ThenProjectionsShouldExistInPersistandStore()
		{
			// Arrange
			using (var inMemoryStore = new EmbeddableDocumentStore { RunInMemory = true })
			{
				inMemoryStore.Initialize();

				using (var session = inMemoryStore.OpenSession())
				{
					session.Store(new Visitor { Id = Guid.NewGuid(), UtcCreated = new DateTime(2011, 11, 01) });
					session.Store(new Visitor { Id = Guid.NewGuid(), UtcCreated = new DateTime(2012, 01, 01) });
					session.Store(new Visitor { Id = Guid.NewGuid(), UtcCreated = new DateTime(2012, 02, 01) });
					session.SaveChanges();
				}

				using (var persistantStore = new EmbeddableDocumentStore { RunInMemory = true })
				{
					persistantStore.Initialize();

					var storer = new ProjectionStorer(inMemoryStore, persistantStore);

					// Act
					storer.PersistAll<Visitor>();

					// Assert
					int nrOfVisitors;
					using (var session = persistantStore.OpenSession())
					{
						nrOfVisitors = session
							.Query<Visitor>()
							.Customize(q => q.WaitForNonStaleResultsAsOfNow())
							.Count();
					}

					Assert.AreEqual(3, nrOfVisitors);
				}
			}
		}
	}
}
