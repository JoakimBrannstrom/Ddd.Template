using Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery;

namespace Ddd.Template.Denormalizer.Rebuilder
{
	internal sealed class ProjectionsRebuilder
	{
		private readonly IEventStorage _eventStore;
		private readonly HandlerInvoker _handlerInvoker;

		public ProjectionsRebuilder(IEventStorage eventStore, HandlerInvoker handlerInvoker)
		{
			_eventStore = eventStore;
			_handlerInvoker = handlerInvoker;
		}

		public int Rebuild()
		{
			var events = _eventStore.GetAll();

			var eventCounter = 0;
			foreach (var @event in events)
			{
				_handlerInvoker.CallHandlers(@event);
				eventCounter++;
			}

			return eventCounter;
		}
	}
}
