using System;
using System.Linq;
using Raven.Client;

namespace Ddd.Template.Projections.Rebuilder
{
	internal sealed class ProjectionStorer
	{
		private readonly IDocumentStore _inMemoryStore;
		private readonly IDocumentStore _persistantStore;

		public ProjectionStorer(IDocumentStore inMemoryStore, IDocumentStore persistantStore)
		{
			_inMemoryStore = inMemoryStore;
			_persistantStore = persistantStore;
		}

		public int PersistAll<T>(int pageSize = 1024) where T : ProjectionInformation
		{
			if (pageSize < 1)
				throw new ArgumentOutOfRangeException("pageSize", pageSize, "Doh!");

			var nrOfProcessedProjections = 0;

			using (var persistantsession = _persistantStore.OpenSession())
			{
				while (true)
				{
					var projections = GetProjections<T>(pageSize, nrOfProcessedProjections);

					if (!projections.Any())
					{
						persistantsession.SaveChanges();
						return nrOfProcessedProjections;
					}

					foreach (var projection in projections)
					{
						persistantsession.Store(projection);
					}

					nrOfProcessedProjections += projections.Length;
				}
			}
		}

		private T[] GetProjections<T>(int pageSize, int nrOfProcessedProjections) where T : ProjectionInformation
		{
			using (var inMemorysession = _inMemoryStore.OpenSession())
			{
				return inMemorysession
						.Query<T>()
						.OrderBy(p => p.UtcCreated)
						.Skip(nrOfProcessedProjections)
						.Take(pageSize)
						.ToArray();
			}
		}
	}
}
