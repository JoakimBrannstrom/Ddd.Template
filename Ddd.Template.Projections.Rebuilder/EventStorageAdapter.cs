using System;
using System.Collections.Generic;
using System.Linq;
using NEventStore;

namespace Ddd.Template.Projections.Rebuilder
{
	internal interface IEventStorage
	{
		IEnumerable<object> GetAll();
	}

	internal sealed class EventStorageAdapter : IEventStorage
	{
		private readonly IStoreEvents _eventStore;

		public EventStorageAdapter(IStoreEvents eventStore)
		{
			_eventStore = eventStore;
		}

		public IEnumerable<object> GetAll()
		{
			var commits = _eventStore.Advanced.GetFrom(DateTime.MinValue);

			return commits.SelectMany(c => c.Events.Select(e => e.Body));
		}
	}
}
