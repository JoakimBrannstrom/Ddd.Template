using System;

namespace Ddd.Template.Projections
{
	public class ProjectionInformation
	{
		public Guid Id { get; set; }
		public int Version { get; set; }
		public DateTime UtcCreated { get; set; }
		public DateTime UtcModified { get; set; }
	}

	public interface IHaveOwner
	{
		Guid OwnerId { get; set; }
	}
}
