using Ddd.Template.Contracts.Commands.Visitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Server.Tests.Contracts.Serialization.Visitor
{
	[TestClass]
	public abstract class VisitorCommandHasSerializableProperties<T> : SerializationTest<T> where T : VisitorCommand
	{
	}
}
