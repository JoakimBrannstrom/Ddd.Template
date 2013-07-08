using System;
using Ddd.Template.Contracts.Events;
using EventStore;
using EventStore.Dispatcher;
using NServiceBus;

namespace Ddd.Template.Server.CommitDispatchers
{
	internal class NServiceBusPublisher : IDispatchCommits
	{
		// private const string AggregateIdKey = "AggregateId";
		// private const string CommitVersionKey = "CommitVersion";
		// private const string EventVersionKey = "EventVersion";
		// private const string BusPrefixKey = "Bus.";

		private readonly RavenDomainViewDispatcher _ravenDispatcher;

		private readonly IBus _bus;
		protected virtual IBus Bus { get { return _bus; } }

		public NServiceBusPublisher(IBus bus, RavenDomainViewDispatcher ravenDispatcher)
		{
			_bus = bus;
			_ravenDispatcher = ravenDispatcher;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public void Dispatch(Commit commit)
		{
			for (var i = 0; i < commit.Events.Count; i++)
			{
				var eventMessage = commit.Events[i];
				var busMessage = eventMessage.Body as Event;
				// AppendHeaders(busMessage, commit.Headers); // optional
				// AppendHeaders(busMessage, eventMessage.Headers); // optional
				// AppendVersion(commit, i); // optional

				_ravenDispatcher.Publish(busMessage);
				Bus.Publish(busMessage);
			}
		}
		/*
		private static void AppendHeaders(IMessage message, IEnumerable<KeyValuePair<string, object>> headers)
		{
			headers = headers.Where(x => x.Key.StartsWith(BusPrefixKey));

			foreach (var header in headers)
			{
				var key = header.Key.Substring(BusPrefixKey.Length);
				var value = (header.Value ?? string.Empty).ToString();

				message.SetHeader(key, value);
			}
		}

		private static void AppendVersion(Commit commit, int index)
		{
			var busMessage = commit.Events[index].Body as IMessage;

			busMessage.SetHeader(AggregateIdKey, commit.StreamId.ToString());
			busMessage.SetHeader(CommitVersionKey, commit.StreamRevision.ToString());
			busMessage.SetHeader(EventVersionKey, GetSpecificEventVersion(commit, index).ToString());
		}

		private static int GetSpecificEventVersion(Commit commit, int index)
		{
			// e.g. (StreamRevision: 120) - (5 events) + 1 + (index @ 4: the last index) = event version: 120
			return commit.StreamRevision - commit.Events.Count + 1 + index;
		}
*/
	}
}