using Ddd.Template.Contracts.Events;
using Raven.Client;

namespace Ddd.Template.Server.CommitDispatchers
{
	public class RavenDomainViewDispatcher
	{
		private readonly IDocumentStore _store;

		public RavenDomainViewDispatcher(IDocumentStore store)
		{
			_store = store;
		}

		public void Publish(Event @event)
		{
			/*
			var linkWasAdded = @event as LinkWasAdded;
			if (linkWasAdded != null)
			{
				StoreLink(linkWasAdded);
			}

			var userWasAdded = @event as UserWasAdded;
			if (userWasAdded != null)
			{
				StoreUser(userWasAdded);
			}
			*/
		}
		/*
		private void StoreLink(LinkWasAdded linkWasAdded)
		{
			using (var session = _store.OpenSession())
			{
				session.Store(new ExistingLink { Id = linkWasAdded.AggregateId, Url = linkWasAdded.Url });
				session.SaveChanges();
			}
		}

		private void StoreUser(UserWasAdded userWasAdded)
		{
			using (var session = _store.OpenSession())
			{
				session.Store(new ExistingUser { Id = userWasAdded.AggregateId, ClaimedIdentifier = userWasAdded.ClaimedIdentifier });
				session.SaveChanges();
			}
		}
		*/
	}
}
