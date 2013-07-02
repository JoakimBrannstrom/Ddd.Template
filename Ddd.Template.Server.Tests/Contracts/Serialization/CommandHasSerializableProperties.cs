using System;
using System.Collections.Generic;
using System.Globalization;
using Ddd.Template.Contracts.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Ddd.Template.Server.Tests.Contracts.Serialization
{
	[TestClass]
	public abstract class CommandHasSerializableProperties<T> where T : Command
	{
		[TestMethod]
		public void AllCommandsShouldHaveTheSerializableAttributeAssigned()
		{
			// Arrange
			var expected = GetExpected();

			// Act
			var json = JsonConvert.SerializeObject(expected);
			var actual = JsonConvert.DeserializeObject<T>(json);

			// Assert
			AssertProperties(expected, actual);
		}

		protected abstract T GetExpected();

		protected virtual void AssertProperties(T expected, T actual)
		{
			Assert.AreEqual(expected.AggregateId, actual.AggregateId);
			Assert.AreEqual(expected.CommandId, actual.CommandId);
			Assert.AreEqual(expected.OriginalVersion, actual.OriginalVersion);
		}

		protected static void Verify(ICollection<string> expected, ICollection<string> actual)
		{
			Assert.AreEqual(expected.Count, actual.Count);

			foreach (var item in expected)
			{
				Assert.IsTrue(actual.Contains(item));
			}
		}

		protected static void Verify(DateTime expected, DateTime actual)
		{
			Assert.AreEqual(expected.ToString(CultureInfo.InvariantCulture), actual.ToString(CultureInfo.InvariantCulture));
		}
	}
}
