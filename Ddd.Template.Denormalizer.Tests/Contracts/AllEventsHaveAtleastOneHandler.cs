using System;
using System.Collections.Generic;
using System.Linq;
using Ddd.Template.Contracts.Events;
using Ddd.Template.Denormalizer.Raven;
using Ddd.Template.Test.Contrib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NServiceBus;

namespace Ddd.Template.Denormalizer.Tests.Contracts
{
	[TestClass]
	public sealed class AllEventsHaveAtleastOneHandler
	{
		// https://gist.github.com/MarkNijhof/Fohjin/blob/master/Fohjin.DDD.Example/Test.Fohjin.DDD/Commands/All_commands_must_have_a_handler.cs
		[TestMethod]
		public void VerifyThatEachEventasAtleastOneEventHandler()
		{
			var commands = TypeFinder.GetAssignableTypes(typeof(Event)).ToList();

			var commandsWithoutHandler = GetEventsWithoutHandler(commands);

			Assert.IsFalse(commandsWithoutHandler.ToArray().Any());
		}

		private IEnumerable<Type> GetEventsWithoutHandler(IEnumerable<Type> events)
		{
			foreach (var @event in events)
			{
				if (@event.IsAbstract)
					continue;	// hmm, should we expect handlers for abstract types?

				if (HandlerExists(@event) == false)
					yield return @event;
			}
		}

		private bool HandlerExists(Type @event)
		{
			// http://stackoverflow.com/questions/326285/deciding-on-type-in-the-runtime-and-applying-it-in-generic-type-how-can-i-do-t
			var specificHandler = typeof(IHandleMessages<>).MakeGenericType(@event);

			return TypeFinder.GetAssignableTypes(specificHandler, typeof(DenormalizerBase).Assembly).Any();
		}
	}
}
