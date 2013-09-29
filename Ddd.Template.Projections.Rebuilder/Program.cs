using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Ddd.Template.Denormalizer.Raven;
using Ddd.Template.Projections.Rebuilder.HandlerDiscovery;
using NEventStore;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Extensions;

namespace Ddd.Template.Projections.Rebuilder
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	static class Program
	{
		static void Main()
		{
			Console.Title = ".:: Ddd.Template.Projections.Rebuilder ::.";

			var inMemoryStore = GetInMemoryStore();

			var persistentStore = GetPersistentStore();

			var storageAdapter = new EventStorageAdapter(BuildEventStore());

			while (true)
			{
				ShowAvailableCommands();

				var cmd = GetUserCommand();

				ExecuteUserCommand(cmd, ref inMemoryStore, persistentStore, storageAdapter);
			}
		}

		private static EmbeddableDocumentStore GetInMemoryStore()
		{
			var inMemoryStore = new EmbeddableDocumentStore { RunInMemory = true, ResourceManagerId = Guid.NewGuid() };
			inMemoryStore.Initialize();
			return inMemoryStore;
		}

		private static DocumentStore GetPersistentStore()
		{
			var persistentStore = new DocumentStore
			{
				ConnectionStringName = "PersistentRavenDb",
				ResourceManagerId = Guid.NewGuid()
			};
			persistentStore.Initialize();

			var persistentRavenDbName = ConfigurationManager.AppSettings["PersistentRavenDbName"];
			if (string.IsNullOrEmpty(persistentRavenDbName))
				throw new Exception("'PersistentRavenDbName' is missing from the app settings!");

			persistentStore.DatabaseCommands.EnsureDatabaseExists(persistentRavenDbName);
			return persistentStore;
		}

		private static IStoreEvents BuildEventStore()
		{
			return Wireup
					.Init()
				//.LogToConsoleWindow()
					.UsingRavenPersistence("RavenDb")
						.ConsistentQueries()
						.PageEvery(int.MaxValue)
						.InitializeStorageEngine()
						.UsingJsonSerialization()
					.Build();
		}

		private static void ShowAvailableCommands()
		{
			Console.WriteLine("To replay events, press 'R'");
			Console.WriteLine("To clear cache of replayed events, press 'C'");
			Console.WriteLine("To save rebuilt projections, press 'S'");
			Console.WriteLine("To exit, press 'Q'");
		}

		private static string GetUserCommand()
		{
			var cmd = Console.ReadKey().Key.ToString().ToLower();
			Console.WriteLine("");
			return cmd;
		}

		private static void ExecuteUserCommand(string cmd, ref EmbeddableDocumentStore inMemoryStore, DocumentStore persistentStore, EventStorageAdapter storageAdapter)
		{
			switch (cmd)
			{
				case "q":
					Console.WriteLine("Bye bye!");
					Environment.Exit(0);
					break;
				case "c":
					Console.WriteLine("Cleaning cache...");
					inMemoryStore = GetInMemoryStore();
					break;
				case "r":
					Console.WriteLine("Replay of all events...");
					ReplayEvents(inMemoryStore, storageAdapter);
					break;
				case "s":
					Console.WriteLine("Saving replayed events...");
					SaveProjections(inMemoryStore, persistentStore);
					break;
			}
		}

		#region ReplayEvents
		private static void ReplayEvents(IDocumentStore store, IEventStorage storageAdapter)
		{
			var handlerInvoker = new HandlerInvoker(GetHandlers(store));

			var projectionsRebuilder = new ProjectionsRebuilder(storageAdapter, handlerInvoker);

			var timer = new Stopwatch();
			timer.Start();
			var nrOfProcessedEvents = projectionsRebuilder.Rebuild();
			timer.Stop();

			int visitorCount;
			GetProjectionsCount(store, out visitorCount);

			WriteDenormalizationResult(timer, nrOfProcessedEvents, visitorCount);
		}

		private static object[] GetHandlers(IDocumentStore store)
		{
			return new object[]
			       	{
						new VisitorDenormalizer(store)
			       	};
		}

		private static void GetProjectionsCount(IDocumentStore store, out int visitorCount)
		{
			using (var session = store.OpenSession())
			{
				visitorCount = session
								.Query<Visitor>()
								.Customize(q => q.WaitForNonStaleResultsAsOfLastWrite())
								.Count();
			}
		}

		private static void WriteDenormalizationResult(Stopwatch timer, int nrOfProcessedEvents, int visitorCount)
		{
			Console.WriteLine("");
			Console.WriteLine("Denormalized view was rebuilt, time: " + timer.Elapsed);
			Console.WriteLine("Nr of processed events: " + nrOfProcessedEvents);
			Console.WriteLine("");
			Console.WriteLine("Nr of visitors: " + visitorCount);
			Console.WriteLine("");
		}
		#endregion

		#region SaveProjections
		private static void SaveProjections(IDocumentStore inMemoryStore, IDocumentStore persistentStore)
		{
			var storer = new ProjectionStorer(inMemoryStore, persistentStore);

			var timer = new Stopwatch();
			timer.Start();

			PersistProjections<Visitor>(storer);

			timer.Stop();
			Console.WriteLine("All projections persisted, time: " + timer.Elapsed);
			Console.WriteLine();
		}

		private static void PersistProjections<T>(ProjectionStorer storer) where T : ProjectionInformation
		{
			Console.Write("Persisting {0}s... ", typeof(T).Name);

			var timer = new Stopwatch();
			timer.Start();
			storer.PersistAll<T>();
			timer.Stop();

			Console.WriteLine(timer.Elapsed);
		}
		#endregion
	}
}
