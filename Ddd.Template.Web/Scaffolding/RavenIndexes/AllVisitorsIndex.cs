using System.Linq;
using Ddd.Template.Projections;
using Raven.Client.Indexes;

namespace Ddd.Template.Web.Scaffolding.RavenIndexes
{
	internal sealed class VisitorCount
	{
		public string Platform { get; set; }
		public int Count { get; set; }
	}

	internal sealed class AllVisitorsIndex : AbstractIndexCreationTask<Visitor, VisitorCount>
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