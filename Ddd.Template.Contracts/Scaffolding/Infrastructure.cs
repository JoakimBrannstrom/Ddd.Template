using System;
using System.Dynamic;
using System.Reflection;

// https://github.com/gregoryyoung/m-r/tree/master/SimpleCQRS
namespace Ddd.Template.Contracts.Scaffolding
{
	// FROM http://blogs.msdn.com/b/davidebb/archive/2010/01/18/use-c-4-0-dynamic-to-drastically-simplify-your-private-reflection-code.aspx
	internal sealed class PrivateReflectionDynamicObject : DynamicObject
	{
		private object RealObject { get; set; }

		internal static object WrapObjectIfNeeded(object o)
		{
			// Don't wrap primitive types, which don't have many interesting internal APIs
			if (o == null || o.GetType().IsPrimitive || o is string)
				return o;

			return new PrivateReflectionDynamicObject { RealObject = o };
		}

		// Called when a method is called
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			result = InvokeMemberOnType(RealObject.GetType(), RealObject, binder.Name, args);

			// Wrap the sub object if necessary. This allows nested anonymous objects to work.
			result = WrapObjectIfNeeded(result);

			return true;
		}

		private static object InvokeMemberOnType(Type type, object target, string name, object[] args)
		{
			try
			{
				const BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

				// Try to invoke the method
				return type.InvokeMember(name, bindingFlags, null, target, args);
			}
			catch (MissingMethodException)
			{
				// If we couldn't find the method, try on the base class
				if (type.BaseType != null)
					return InvokeMemberOnType(type.BaseType, target, name, args);

				// quick greg hack to allow methods to not exist!
				return null;
			}
		}
	}

	public static class PrivateReflectionDynamicObjectExtensions
	{
		public static dynamic AsDynamic(this object o)
		{
			return PrivateReflectionDynamicObject.WrapObjectIfNeeded(o);
		}
	}
}
