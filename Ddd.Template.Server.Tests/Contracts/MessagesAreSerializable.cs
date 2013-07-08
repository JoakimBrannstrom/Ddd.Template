using System.Linq;
using Ddd.Template.Contracts.Commands;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Test.Contrib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Server.Tests.Contracts
{
	[TestClass]
	public sealed class MessagesAreSerializable
	{
		[TestMethod]
		public void AllCommandsShouldHaveTheSerializableAttributeAssigned()
		{
			var nonSerializableTypes = TypeFinder.GetNonSerializableTypes(typeof(Command)).ToArray();

			var message = string.Format("The following commands are not serializable: {0}",
										string.Join(", ", nonSerializableTypes.Select(t => t.Name)));

			Assert.IsFalse(nonSerializableTypes.Any(), message);
		}

		[TestMethod]
		public void AllEventsShouldHaveTheSerializableAttributeAssigned()
		{
			var nonSerializableTypes = TypeFinder.GetNonSerializableTypes(typeof(Event)).ToArray();

			var message = string.Format("The following events are not serializable: {0}",
										string.Join(", ", nonSerializableTypes.Select(t => t.Name)));

			Assert.IsFalse(nonSerializableTypes.Any(), message);
		}
	}
}
