using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ddd.Template.Domain.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;

namespace Ddd.Template.Server.Tests.Contracts
{
	[TestClass]
	public sealed class AllCommandHandlerMethodsShouldHaveAtleastOneTest
	{
		[TestMethod]
		public void VerifyThatEachCommandHandlerMethodHaveAtLeastOneTest()
		{
			// var commandHandlers = AppDomain.CurrentDomain.GetAssemblies().ToArray().SelectMany(s => s.GetTypes())
			var commandHandlers = typeof(CommandHandlerBase<>).Assembly.GetTypes()
									.Where(t => GetMessageHandlerContracts(t).Any())
									.ToArray();

			var errorMessages = new StringBuilder();

			foreach (var handler in commandHandlers)
			{
				var contracts = GetMessageHandlerContracts(handler);

				var testFixtures = GetTestFixtures(GetType().Assembly.GetTypes());

				var methodsWithoutTests = GetMethodsWithoutTests(handler, contracts, testFixtures).ToArray();

				var message = string.Format("{0}{1}{0}",
											Environment.NewLine,
											string.Join(Environment.NewLine, methodsWithoutTests.Select(GetMethodSignature)));

				if (methodsWithoutTests.Any())
					errorMessages.Append(message);
			}

			if (errorMessages.Length > 0)
				Assert.Fail("The following command handler methods are not covered by tests:" + Environment.NewLine + errorMessages);
		}

		private static IEnumerable<Type> GetMessageHandlerContracts(Type messageHandlerCandidate)
		{
			var messageHandler = typeof(IMessageHandler<>);

			var interfaces = messageHandlerCandidate.GetInterfaces();

			return interfaces
					.Where(i => i.GenericTypeArguments.Length == 1)
					.Where(i => i == messageHandler.MakeGenericType(i.GenericTypeArguments.First()));
		}

		private IEnumerable<Type> GetTestFixtures(IEnumerable<Type> testFixtureCandidates)
		{
			foreach (var candidate in testFixtureCandidates)
			{
				var baseType = candidate.BaseType;

				while (baseType != null)
				{
					if (baseType.GenericTypeArguments.Length == 3)
					{
						var specificTestFixture = typeof(CommandTestFixture<,,>).MakeGenericType(baseType.GenericTypeArguments);
						if (baseType == specificTestFixture)
							yield return candidate;
					}

					baseType = baseType.BaseType;
				}
			}
		}

		private IEnumerable<MethodInfo> GetMethodsWithoutTests(Type handler, IEnumerable<Type> implementedContracts, IEnumerable<Type> testFixtures)
		{
			var aggregateType = GetHandlerAggregate(handler);

			var fixtures = testFixtures
							.Where(f => GetAggregate(f) == aggregateType)	// Find the tests for this aggregate type
							.Where(f => GetCommandHandler(f) == handler)	// that uses this handler type
							.ToArray();

			foreach (var contract in implementedContracts)
			{
				var contract1 = contract;
				var command = contract1.GenericTypeArguments.First();

				if (fixtures.All(f => GetCommand(f) != command))
				{
					yield return handler
									.GetInterfaceMap(contract1)
									.TargetMethods
									.First();
				}
			}
		}

		private static Type GetHandlerAggregate(Type handlerType)
		{
			var commandHandler = GetTypeFromHierarchy(handlerType, typeof(CommandHandlerBase<>), 1);

			if (commandHandler != null)
				return commandHandler.GenericTypeArguments.First();

			return null;
		}

		private static Type GetTypeFromHierarchy(Type subType, Type typeToLookFor, int nrOfGenericTypes)
		{
			var baseType = subType;

			while (baseType != null)
			{
				if (baseType.GenericTypeArguments.Length == nrOfGenericTypes
					&& baseType == typeToLookFor.MakeGenericType(baseType.GenericTypeArguments))
					return baseType;

				baseType = baseType.BaseType;
			}

			return baseType;
		}

		private static Type GetAggregate(Type fixture)
		{
			var baseFixture = GetTypeFromHierarchy(fixture, typeof(CommandTestFixture<,,>), 3);

			if (baseFixture != null)
				return baseFixture.GenericTypeArguments.First();

			return null;
		}

		private static Type GetCommand(Type fixture)
		{
			var baseFixture = GetTypeFromHierarchy(fixture, typeof(CommandTestFixture<,,>), 3);

			if (baseFixture != null)
				return baseFixture.GenericTypeArguments.Skip(1).First();

			return null;
		}

		private static Type GetCommandHandler(Type fixture)
		{
			var baseFixture = GetTypeFromHierarchy(fixture, typeof(CommandTestFixture<,,>), 3);

			if (baseFixture != null)
				return baseFixture.GenericTypeArguments.Skip(2).First();

			return null;
		}

		private static string GetMethodSignature(MethodInfo t)
		{
			var name = "[Unknown DeclaringType]";

			if (t.DeclaringType != null)
				name = t.DeclaringType.Name;

			return string.Format("\t* {0}::{1}({2})", name, t.Name, GetParameters(t));
		}

		private static string GetParameters(MethodInfo t)
		{
			return string.Join(", ", t.GetParameters().Select(p => string.Format("{0} {1}", p.ParameterType.Name, p.Name)));
		}
	}
}
