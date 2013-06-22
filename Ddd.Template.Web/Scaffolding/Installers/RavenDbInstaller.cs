using System;
using System.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DocumentStation.Web.Scaffolding.RavenIndexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Client.Indexes;

namespace DocumentStation.Web.Scaffolding.Installers
{
	public class RavenDbInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.AddFacility<RavenDbFacility>();
		}
	}

	public class RavenDbFacility : AbstractFacility
	{
		private string _projectionsDbName;

		protected override void Init()
		{
			SetDbName(ConfigurationManager.AppSettings["RavenDbName"]);

			Kernel.Register(Component
								.For<IDocumentStore>()
								.ImplementedBy<DocumentStore>()
								.DependsOn(new
								{
									ConnectionStringName = Settings.RavenDbConnectionStringName,
									ResourceManagerId = Guid.NewGuid()
								})
								.OnCreate(InitializeDocumentStore)
								.LifeStyle.Singleton);

			Kernel.Register(Component
								.For<IDocumentSession>()
								.UsingFactoryMethod(GetDocumentSesssion)
								.LifeStyle.Transient);
		}

		public void SetDbName(string dbName)
		{
			_projectionsDbName = dbName;
		}

		static IDocumentSession GetDocumentSesssion(IKernel kernel)
		{
			var store = kernel.Resolve<IDocumentStore>();
			return store.OpenSession();
		}

		public void InitializeDocumentStore(IKernel kernel, IDocumentStore store)
		{
			store.Initialize();

			store.DatabaseCommands.EnsureDatabaseExists(_projectionsDbName);

			// https://github.com/ravendb/ravendb/blob/master/Raven.Tests/Bugs/TransformResults/WithGuidId.cs
			// store.Conventions.FindFullDocumentKeyFromNonStringIdentifier = (entityId, type, allowNull) => entityId.ToString();

			// store.Conventions.MaxNumberOfRequestsPerSession = 200;

			// http://groups.google.com/group/ravendb/browse_thread/thread/8a3effa13d6bac03
			// store.Conventions.DocumentKeyGenerator = entity => null; 

			// http://groups.google.com/group/ravendb/browse_thread/thread/ee2f7725da6d6108
			// store.Conventions.IdentityTypeConvertors.Clear();
			// store.Conventions.IdentityTypeConvertors.Add(new GuidConverter());

			// store.Conventions.DocumentKeyGenerator = a => Guid.NewGuid().ToString();
			// store.Conventions.MaxNumberOfRequestsPerSession = 200;

			// Create all indexes that is defined in the same assembly as 'AllTagsIndex'
			IndexCreation.CreateIndexes(typeof(AllTagsIndex).Assembly, store);
			/*
			using (var session = store.OpenSession())
			{
				session.Store(new FacetSetup
				{
					Id = Settings.VisitorTimeFacet.Name,
					Facets = new List<Facet>
					         	{
					         		new Facet { Name = Settings.VisitorTimeFacet.Year },
					         		new Facet { Name = Settings.VisitorTimeFacet.YearAndMonth },
					         		new Facet { Name = Settings.VisitorTimeFacet.YearAndMonthAndDay }
					         	}
				});

				session.SaveChanges();
			}
			*/

			// RavenProfiler.InitializeFor(store);
		}
	}
}
