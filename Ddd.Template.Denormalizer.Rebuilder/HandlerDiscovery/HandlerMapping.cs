using System;
using System.Collections.Generic;
using System.Linq;

namespace Ddd.Template.Denormalizer.Rebuilder.HandlerDiscovery
{
	internal sealed class HandlerMapping
	{
		public Type HandlerType { get; private set; }
		public List<HandlerMessageMapping> MessageMappings { get; private set; }

		public HandlerMapping(Type handlerType, IEnumerable<HandlerMessageMapping> messageMappings)
		{
			HandlerType = handlerType;
			MessageMappings = messageMappings.ToList();
		}
	}
}
