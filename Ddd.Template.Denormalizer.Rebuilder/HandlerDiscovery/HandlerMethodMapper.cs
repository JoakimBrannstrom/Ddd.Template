using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NServiceBus;

namespace Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery
{
	internal sealed class HandlerMethodMapper
	{
		private readonly MessageTypeFinder _messageTypeFinder = new MessageTypeFinder();

		public IEnumerable<HandlerMessageMapping> Map(Type handler)
		{
			var messageTypes = MessageTypeFinder.GetHandledMessageTypes(handler);

			foreach (var messageType in messageTypes)
			{
				var methodInfo = GetMessageHandlerMethodInfo(handler, messageType);

				if (methodInfo != null)
					yield return new HandlerMessageMapping(messageType, methodInfo);
			}
		}

		private static MethodInfo GetMessageHandlerMethodInfo(Type handler, Type messageType)
		{
			var handlerInterface = typeof(IMessageHandler<>).MakeGenericType(messageType);

			if (!handlerInterface.IsAssignableFrom(handler))
				return null;

			return handler
					.GetInterfaceMap(handlerInterface)
					.TargetMethods
					.FirstOrDefault();
		}
	}
}
