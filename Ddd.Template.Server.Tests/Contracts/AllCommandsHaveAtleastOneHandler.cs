using System;
using System.Collections.Generic;
using System.Linq;
using Ddd.Template.Contracts.Commands;
using Ddd.Template.Domain.Aggregates;
using Ddd.Template.Test.Contrib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;

namespace Ddd.Template.Server.Tests.Contracts
{
	[TestClass]
	public class AllCommandsHaveAtleastOneHandler
	{
		// https://gist.github.com/MarkNijhof/Fohjin/blob/master/Fohjin.DDD.Example/Test.Fohjin.DDD/Commands/All_commands_must_have_a_handler.cs
		[TestMethod]
		public void VerifyThatEachCommandHasAtleastOneCommandHandler()
		{
			var commands = TypeFinder.GetAssignableTypes(typeof(Command));

			var commandsWithoutHandler = GetCommandsWithoutHandler(commands).ToArray();

			var message = string.Format("The following commands are missing a command handler: {0}",
										string.Join(", ", commandsWithoutHandler.Select(t => t.Name)));

			Assert.IsFalse(commandsWithoutHandler.ToArray().Any(), message);
		}

		private IEnumerable<Type> GetCommandsWithoutHandler(IEnumerable<Type> commands)
		{
			foreach (var command in commands)
			{
				if (command.IsAbstract)
					continue;	// hmm, should we expect handlers for abstract types?

				if (HandlerExists(command) == false)
					yield return command;
			}
		}

		private bool HandlerExists(Type command)
		{
			// http://stackoverflow.com/questions/326285/deciding-on-type-in-the-runtime-and-applying-it-in-generic-type-how-can-i-do-t
			var specificHandler = typeof(IHandleMessages<>).MakeGenericType(command);

			return TypeFinder.GetAssignableTypes(specificHandler, typeof(Visitor).Assembly).Any();
		}
	}
}
