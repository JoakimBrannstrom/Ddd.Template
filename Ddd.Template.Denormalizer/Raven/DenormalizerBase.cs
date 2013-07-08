using System;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Projections;
using Raven.Client;

namespace Ddd.Template.Denormalizer.Raven
{
	public abstract class DenormalizerBase
	{
		private readonly IDocumentStore _store;

		protected DenormalizerBase(IDocumentStore store)
		{
			_store = store;
		}

		protected void HandleEventIfVersionIsSubsequent<T>(Event @event, Action<T> action) where T : ProjectionInformation, new()
		{
			using (var session = _store.OpenSession())
			{
				var originatorId = Guid.Empty;
				var originatorEvent = @event as IHaveOriginator;
				if (originatorEvent != null)
					originatorId = originatorEvent.OriginatorId;

				var projection = GetProjection<T>(session, @event.AggregateId, originatorId);

				var subsequentVersion = projection.Version + 1;
				if (@event.Version != subsequentVersion)
				{
					var message = string.Format("@event.Version ({0}) was not subsequent to existing.Version ({1})",
												@event.Version, projection.Version);

					throw new ArgumentOutOfRangeException("event", message);
				}

				projection.Version = subsequentVersion;
				projection.UtcModified = @event.UtcTimestamp;
				action(projection);

				session.Store(projection);
				session.SaveChanges();
			}
		}

		private static T GetProjection<T>(IDocumentSession session, Guid id, Guid originatorId) where T : ProjectionInformation, new()
		{
			var projection = session.Load<T>(id);

			if (projection != null)
				return projection;

			projection = new T { Id = id, Version = -1 };

			var originatorProjection = projection as IHaveOwner;
			if (originatorProjection != null)
				originatorProjection.OwnerId = originatorId;

			return projection;
		}
	}
}
