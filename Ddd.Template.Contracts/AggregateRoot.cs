using System;
using System.Collections.Generic;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Scaffolding;

namespace Ddd.Template.Contracts
{
	public abstract class AggregateRoot
	{
		private readonly List<Event> _changes = new List<Event>();

		public abstract Guid Id { get; }

		public IEnumerable<Event> GetUncommittedChanges()
		{
			return _changes;
		}

		public void MarkChangesAsCommitted()
		{
			_changes.Clear();
		}

		public void LoadFromHistory(IEnumerable<Event> history)
		{
			foreach (var e in history)
				ApplyChange(e, false);
		}

		protected void ApplyChange(Event @event)
		{
			ApplyChange(@event, true);
		}

		private void ApplyChange(Event @event, bool isNew)
		{
			this.AsDynamic().Apply(@event);

			if (isNew)
				_changes.Add(@event);
		}
	}
}
