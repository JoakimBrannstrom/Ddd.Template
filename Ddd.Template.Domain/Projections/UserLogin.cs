using System;

namespace Ddd.Template.Domain.Projections
{
	public class UserLogin
	{
		public Guid Id { get; set; }
		public string ClaimedIdentifier { get; set; }
	}
}
