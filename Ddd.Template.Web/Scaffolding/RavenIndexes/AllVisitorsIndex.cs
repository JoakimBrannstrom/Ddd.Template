using System.Linq;
using Raven.Client.Indexes;
using Ddd.Template.Denormalizer.Projections;

namespace Ddd.Template.Web.Scaffolding.RavenIndexes
{
	public class VisitorCount
	{
		public string Platform { get; set; }
		public int Count { get; set; }
	}

	public class AllVisitorsIndex : AbstractIndexCreationTask<Visitor, VisitorCount>
	{
		public AllVisitorsIndex()
		{
			Map = visitors =>	from visitor in visitors
								select new VisitorCount { Platform = visitor.Platform, Count = 1 };

			Reduce = results => from result in results
								group result by result.Platform into g
								select new
								{
									Platform = g.Key,
									Count = g.Sum(x => x.Count)
								};
		}
	}
}