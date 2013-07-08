using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ddd.Template.Server.CommitDispatchers;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Logging;

namespace Ddd.Template.Server.Bootstrap
{
	public sealed class EventStoreInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(Component
								.For<RavenDomainViewDispatcher>()
								.ImplementedBy(typeof(RavenDomainViewDispatcher))
								.LifeStyle.Singleton);

			container.Register(Component
								.For<IDispatchCommits>()
								.ImplementedBy(typeof(NServiceBusPublisher))
								.LifeStyle.Singleton);

			container.Register(Component
								.For<IStoreEvents>()
								.Instance(BuildEventStore(container.Kernel))
								.LifeStyle.Singleton);
		}

		private static IStoreEvents BuildEventStore(IKernel container)
		{
			var publisher = container.Resolve<IDispatchCommits>();

			var eventStore = Wireup
								.Init()
								.UsingRavenPersistence(Settings.RavenDbConnectionStringName)
									.ConsistentQueries()
									.PageEvery(int.MaxValue)
									.InitializeStorageEngine()
										.UsingJsonSerialization()
								.UsingSynchronousDispatchScheduler(publisher)
								.Build();

			LogFactory.BuildLogger = type => new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

			return eventStore;
		}
	}
}
