using System;
using System.Reflection;

namespace Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery
{
	internal sealed class HandlerMessageMapping
	{
		public Type MessageType { get; private set; }
		public MethodInfo HandlingMethod { get; private set; }

		public HandlerMessageMapping(Type messageType, MethodInfo handlingMethod)
		{
			MessageType = messageType;
			HandlingMethod = handlingMethod;
		}
	}
}
