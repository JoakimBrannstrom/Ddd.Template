using System;
using System.Linq;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Domain;
using EventStore;
using ConcurrencyException = Ddd.Template.Contracts.ConcurrencyException;

namespace Ddd.Template.Server
{
	internal class Repository<T> : IRepository<T> where T : AggregateRoot, new()
	{
		private readonly IStoreEvents _eventStore;

		public Repository(IStoreEvents eventStore)
		{
			_eventStore = eventStore;
		}

		public void Save(Guid commitId, AggregateRoot aggregate, int originalVersion, Guid originatorId)
		{
			using (var stream = _eventStore.OpenStream(aggregate.Id, 0, int.MaxValue))
			{
				ValidateStreamRevision(aggregate.Id, originalVersion, commitId, stream.StreamRevision);

				var events = aggregate.GetUncommittedChanges();

				var currentVersion = originalVersion;
				foreach (var @event in events)
				{
					@event.AggregateId = aggregate.Id;
					@event.Version = currentVersion++;
					@event.UtcTimestamp = DateTime.UtcNow;

					var originatorEvent = @event as IHaveOriginator;
					if (originatorEvent != null)
						originatorEvent.OriginatorId = originatorId;

					stream.Add(new EventMessage { Body = @event });
				}

				stream.CommitChanges(commitId);
			}

			aggregate.MarkChangesAsCommitted();
		}

		private static void ValidateStreamRevision(Guid aggregateId, int originalVersion, Guid commitId, int streamRevision)
		{
			if (streamRevision == 0 && originalVersion != 0)
			{
				var message = string.Format("Could not find event stream for aggregate with id '{0}'. (originalVersion: '{1}', commitId: '{2}'",
											aggregateId, originalVersion, commitId);
				throw new AggregateNotFoundException(message);
			}

			if (streamRevision != originalVersion)
			{
				var message = string.Format("Expected version ({0}) did not match event stream revision ({1}).",
											originalVersion, streamRevision);
				throw new ConcurrencyException(message);
			}
		}

		public T GetById(Guid id)
		{
			using (var stream = _eventStore.OpenStream(id, 0, int.MaxValue))
			{
				var history = stream
								.CommittedEvents
								.Select(e => e.Body as Event);

				var aggregate = new T();
				aggregate.LoadFromHistory(history);
				return aggregate;
			}
		}

		public bool Exists(Guid id)
		{
			using (var stream = _eventStore.OpenStream(id, 0, 1))
			{
				return stream.CommittedEvents.Any();
			}
		}
	}
}
