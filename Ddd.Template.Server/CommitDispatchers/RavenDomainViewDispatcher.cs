using Ddd.Template.Contracts.Events;
using Ddd.Template.Contracts.Events.Visitor;
using Ddd.Template.Domain.Projections;
using Raven.Client;

namespace Ddd.Template.Server.CommitDispatchers
{
	internal sealed class RavenDomainViewDispatcher
	{
		private readonly IDocumentStore _store;

		public RavenDomainViewDispatcher(IDocumentStore store)
		{
			_store = store;
		}

		public void Publish(Event @event)
		{
			var visitorLoggedIn = @event as VisitorLoggedIn;
			if (visitorLoggedIn != null)
			{
				StoreLink(visitorLoggedIn);
			}
		}

		private void StoreLink(VisitorLoggedIn loginInfo)
		{
			using (var session = _store.OpenSession())
			{
				session.Store(new UserLogin
								{
									Id = loginInfo.AggregateId,
									ClaimedIdentifier = loginInfo.ClaimedIdentifier
								});
				session.SaveChanges();
			}
		}
	}
}
