using System;
using System.Collections.Generic;
using Ddd.Template.Contracts.Commands.Visitor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Server.Tests.Contracts.Serialization.Visitor
{
	[TestClass]
	public sealed class AddVisitorHasSerializableProperties : VisitorCommandHasSerializableProperties<AddVisitor>
	{
		protected override AddVisitor GetExpected()
		{
			return new AddVisitor
			{
				CommandId = Guid.NewGuid(),
				AggregateId = Guid.NewGuid(),
				OriginalVersion = 1,

				Created = DateTime.Now,
				Platform = "Windows",
				UserAgent = "Mozilla",
				UserHostAddress = "127.0.0.1",
				UserHostName = "localhost",
				UserLanguages = new List<string> { "sv-SE" }
			};
		}

		protected override void AssertProperties(AddVisitor expected, AddVisitor actual)
		{
			Verify(expected.Created, actual.Created);

			Assert.AreEqual(expected.Platform, actual.Platform);
			Assert.AreEqual(expected.UserAgent, actual.UserAgent);
			Assert.AreEqual(expected.UserHostAddress, actual.UserHostAddress);
			Assert.AreEqual(expected.UserHostName, actual.UserHostName);

			Verify(expected.UserLanguages, actual.UserLanguages);

			base.AssertProperties(expected, actual);
		}
	}
}
