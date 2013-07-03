using System;
using Ddd.Template.Contracts.Events;
using NServiceBus;

namespace Ddd.Template.Denormalizer
{
	internal class ConsoleInteraction : IWantToRunAtStartup, IHandleMessages<Event>
	{
		public void Run()
		{
			if (!Environment.UserInteractive)
				return;

			Console.Title = ".:: Ddd.Template.Denormalizer ::.";

			while (true)
			{
				Console.WriteLine("To exit, press 'Q'");
				var cmd = Console.ReadKey().Key.ToString().ToLower();
				Console.WriteLine("");

				switch (cmd)
				{
					case "q":
						Environment.Exit(0);
						break;
				}
			}
		}

		public void Stop()
		{
			Console.WriteLine("Shutting down...");
		}

		public void Handle(Event message)
		{
			if (!Environment.UserInteractive)
				return;

			Console.WriteLine("{0} - Event received, Type: '{1}'", DateTime.Now, message.GetType().Name);
			Console.WriteLine("\tEvent type: '{0}'", message.GetType().Name);
			Console.WriteLine("\tAggregateId: '{0}'", message.AggregateId);
			Console.WriteLine("\tVersion: '{0}'", message.Version);
		}
	}
}
