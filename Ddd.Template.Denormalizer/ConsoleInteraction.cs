using System;
using Ddd.Template.Contracts.Events;
using NServiceBus;

namespace Ddd.Template.Denormalizer
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal sealed class ConsoleInteraction : IWantToRunAtStartup, IHandleMessages<Event>
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

		private static int _messageCounter = 0;
		public void Handle(Event message)
		{
			if (!Environment.UserInteractive)
				return;

			_messageCounter++;
			Console.WriteLine("{0} - Event received.", DateTime.Now);
			Console.WriteLine("\tMessage nr: {0}", _messageCounter);
			Console.WriteLine("\tEvent type: '{0}'", message.GetType().Name);
			Console.WriteLine("\tAggregateId: '{0}'", message.AggregateId);
			Console.WriteLine("\tVersion: '{0}'", message.Version);
		}
	}
}
