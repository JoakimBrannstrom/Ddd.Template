using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Extensions;
using System;

namespace Ddd.Template.Contrib
{
	public static class RavenExtensions
	{
		internal static Action<IDocumentStore, string> DatabaseExistsFactory = (store, name) => store.DatabaseCommands.EnsureDatabaseExists(name);

		public static void EnsureDatabaseExists(this IDocumentStore store, string name)
		{
			DatabaseExistsFactory(store, name);
		}
	}
}
