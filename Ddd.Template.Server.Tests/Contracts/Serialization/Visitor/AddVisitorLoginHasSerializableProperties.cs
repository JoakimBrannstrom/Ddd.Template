using System;
using Ddd.Template.Contracts.Commands.Visitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Server.Tests.Contracts.Serialization.Visitor
{
	[TestClass]
	public class AddVisitorLoginHasSerializableProperties : VisitorCommandHasSerializableProperties<AddVisitorLogin>
	{
		protected override AddVisitorLogin GetExpected()
		{
			return new AddVisitorLogin
				{
					CommandId = Guid.NewGuid(),
					AggregateId = Guid.NewGuid(),
					OriginalVersion = 1,

					UserId = Guid.NewGuid()
				};
		}

		protected override void AssertProperties(AddVisitorLogin expected, AddVisitorLogin actual)
		{
			Assert.AreEqual(expected.UserId, actual.UserId);

			base.AssertProperties(expected, actual);
		}
	}
}
