using System;
using System.Collections.Generic;

namespace Ddd.Template.Denormalizer.Projections
{
	public sealed class Visitor : ProjectionInformation
	{
		public string UserAgent { get; set; }
		public string Platform { get; set; }
		public string UserHostAddress { get; set; }
		public string UserHostName { get; set; }
		public List<string> UserLanguages { get; set; }

		public Guid UserId { get; set; }

		private List<VisitInfo> _visitedLinks;
		public List<VisitInfo> VisitedLinks
		{
			get { return _visitedLinks ?? (_visitedLinks = new List<VisitInfo>()); }
			set { _visitedLinks = value; }
		}
	}

	public sealed class VisitInfo
	{
		public DateTime Timestamp { get; set; }
		public Guid LinkId { get; set; }
	}
}
