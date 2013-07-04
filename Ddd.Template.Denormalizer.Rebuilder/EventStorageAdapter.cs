using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;

namespace Ddd.Template.Denormalizer.Rebuilder
{
	public interface IEventStorage
	{
		IEnumerable<object> GetAll();
	}

	public class EventStorageAdapter : IEventStorage
	{
		private readonly IStoreEvents _eventStore;

		public EventStorageAdapter(IStoreEvents eventStore)
		{
			_eventStore = eventStore;
		}

		public virtual IEnumerable<object> GetAll()
		{
			var commits = _eventStore.Advanced.GetFrom(DateTime.MinValue);

			return commits.SelectMany(c => c.Events.Select(e => e.Body));
		}
	}
}
