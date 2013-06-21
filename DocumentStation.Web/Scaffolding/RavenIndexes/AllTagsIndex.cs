using System.Linq;
using Raven.Client.Indexes;

namespace DocumentStation.Web.Scaffolding.RavenIndexes
{
	public class TagCount
	{
		public string Tag { get; set; }
		public int Count { get; set; }
	}

	public class AllTagsIndex : AbstractIndexCreationTask<object, TagCount>
	{
		/*
		public AllTagsIndex()
		{
			Map = links => from link in links
						   where link.Tags != null
						   from tag in link.Tags
						   select new { Tag = tag.ToLowerInvariant(), Count = 1 };

			Reduce = results => from result in results
								group result by result.Tag into g
								select new
								{
									Tag = g.Key,
									Count = g.Sum(x => x.Count)
								};
		}
		*/
	}
}