using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ddd.Template.Test.Contrib
{
	public static class TypeFinder
	{
		public static IEnumerable<Type> GetNonSerializableTypes(Type baseType)
		{
			var domainEventTypes = GetAssignableTypes(baseType);

			return domainEventTypes
					.Where(eventType => !eventType.IsSerializable);
		}

		public static IEnumerable<Type> GetAssignableTypes(Type baseType)
		{
			return GetAssignableTypes(baseType, baseType.Assembly);
		}

		public static IEnumerable<Type> GetAssignableTypes(Type baseType, Assembly assembly)
		{
			return assembly
					.GetExportedTypes()
					.Where(baseType.IsAssignableFrom);
		}
	}
}
