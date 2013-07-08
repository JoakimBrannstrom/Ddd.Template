using System;
using System.Collections.Generic;
using System.Linq;

namespace Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery
{
	public sealed class HandlerInvoker
	{
		private readonly object[] _handlers;
		private readonly List<HandlerMapping> _handlerMappings;

		public HandlerInvoker(object[] handlers)
		{
			_handlers = handlers;

			var mappings = GetHandlerMappings(_handlers);
			_handlerMappings = new List<HandlerMapping>(mappings);
		}

		private static IEnumerable<HandlerMapping> GetHandlerMappings(IEnumerable<object> handlers)
		{
			var methodMapper = new HandlerMethodMapper();

			foreach (var handler in handlers)
			{
				var methods = methodMapper.Map(handler.GetType());

				yield return new HandlerMapping(handler.GetType(), methods);
			}
		}

		public void CallHandlers(object @event)
		{
			foreach (var handler in _handlers)
			{
				var handlerMapping = _handlerMappings.FirstOrDefault(m => m.HandlerType == handler.GetType());
				if (handlerMapping == null)
					continue;

				var methods = handlerMapping
								.MessageMappings
								.Where(m => MappingIsMatched(m.MessageType, @event))
								.Select(m => m.HandlingMethod)
								.ToArray();

				foreach (var method in methods)
				{
					method.Invoke(handler, new[] { @event });
				}
			}
		}

		private static bool MappingIsMatched(Type handledMessageType, object @event)
		{
			var eventType = @event.GetType();

			// Let's check if the event or one of it's base types is handled...
			while (eventType != null)
			{
				if (handledMessageType == eventType)
					return true;

				eventType = eventType.BaseType;
			}

			return false;
		}
	}
}
