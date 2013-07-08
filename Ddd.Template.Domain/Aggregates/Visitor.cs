using System;
using System.Collections.Generic;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Events.Visitor;

namespace Ddd.Template.Domain.Aggregates
{
	public sealed class Visitor : AggregateRoot
	{
		private Guid _id;

		public override Guid Id { get { return _id; } }

		public Visitor()
		{
		}

		public Visitor(Guid aggregateId, DateTime created, string platform, string userAgent,
						string userHostAddress, string userHostName, List<string> userLanguages)
		{
			ApplyChange(new VisitorArrived
			{
				AggregateId = aggregateId,
				UserAgent = userAgent,
				Created = created,
				UserHostAddress = userHostAddress,
				UserHostName = userHostName,
				UserLanguages = userLanguages,
				Platform = platform
			});
		}

		public void Login(Guid userId)
		{
			ApplyChange(new VisitorLoggedIn
			{
				AggregateId = Id,
				UserId = userId
			});
		}

		#region Handle events
		private void Apply(VisitorArrived @event)
		{
			_id = @event.AggregateId;
		}

		private void Apply(VisitorLoggedIn @event)
		{
			// We don't have to store any internal state for this event
		}
		#endregion
	}
}
