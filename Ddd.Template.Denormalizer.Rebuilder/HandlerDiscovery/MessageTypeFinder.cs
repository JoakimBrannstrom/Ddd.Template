using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;

namespace Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery
{
	internal sealed class MessageTypeFinder
	{
		public static IEnumerable<Type> GetHandledMessageTypes(Type handler)
		{
			var genericInterfaces = GetGenericInterfaces(handler);

			return GetHandlerArguments(genericInterfaces).Distinct();
		}

		private static IEnumerable<Type> GetGenericInterfaces(Type handler)
		{
			return handler
					.GetInterfaces()
					.Where(i => i.IsGenericType);
		}

		private static IEnumerable<Type> GetHandlerArguments(IEnumerable<Type> allGenericInterfacesOnHandler)
		{
			foreach (var potentialHandlerInterface in allGenericInterfacesOnHandler)
			{
				var potentialMessageType = potentialHandlerInterface.GetGenericArguments().SingleOrDefault();

				if (potentialMessageType == null || !IsMessageHandlerInterface(potentialHandlerInterface, potentialMessageType))
					continue;

				yield return potentialMessageType;
			}
		}

		private static bool IsMessageHandlerInterface(Type genericInterface, Type messageType)
		{
			return typeof(IMessageHandler<>).MakeGenericType(messageType).IsAssignableFrom(genericInterface);
		}
	}
}
