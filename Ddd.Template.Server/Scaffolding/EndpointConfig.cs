using System;
using System.Configuration;
using System.IO;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Ddd.Template.Contracts;
using NServiceBus;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using log4net.Config;

namespace Ddd.Template.Server.Scaffolding
{
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
	{
		protected IWindsorContainer Container;

		public void Init()
		{
			Container = BootstrapContainer();

			SetNServiceBusLoggingLibrary();

			Configure configuration = GetConfigurationInstance();

			AddUnbotrusiveConventions(configuration);

			ConfigureNServiceBus(configuration, Container);

			RegisterDocumentStore(Container);

			Container.Install(FromAssembly.This());
		}

		private IWindsorContainer BootstrapContainer()
		{
			var container = new WindsorContainer();

			// http://stw.castleproject.org/Windsor.Windsor-Tutorial-Part-Five-Adding-logging-support.ashx?HL=ilogger
			container
				.AddFacility<LoggingFacility>(f => f.UseLog4Net(Settings.Log4NetConfigurationFilename));

			var logger = container.Resolve<ILogger>();
			logger.Debug("Installing Ddd.Template.Server components");

			return container;
		}

		private void SetNServiceBusLoggingLibrary()
		{
			// http://nservicebus.com/Logging.aspx#customized
			SetLoggingLibrary.Log4Net(() => XmlConfigurator
				                                .Configure(new FileInfo(Settings.Log4NetConfigurationFilename)));
		}

		protected virtual Configure GetConfigurationInstance()
		{
			return Configure.With();
		}

		private static void AddUnbotrusiveConventions(Configure configuration)
		{
			Func<Type, bool> commandTypeDefinition = MessageConfigurator.GetMessageTypeDefinition(MessageType.Command);
			Func<Type, bool> eventTypeDefinition = MessageConfigurator.GetMessageTypeDefinition(MessageType.Event);

			configuration
				.DefiningCommandsAs(commandTypeDefinition)
				.DefiningEventsAs(eventTypeDefinition)
				.DefiningEncryptedPropertiesAs(p => p.Name.StartsWith("Encrypted"));
		}

		private void ConfigureNServiceBus(Configure configuration, IWindsorContainer container)
		{
			configuration
				.DefineEndpointName("Ddd.Template.Domain")
				.CastleWindsorBuilder(container)
				.RavenPersistence("NServiceBus.Persistence", "Ddd.Template.NServiceBus.domain")
				.RunTimeoutManager()
				.UseRavenTimeoutPersister()
				.XmlSerializer()
				.MsmqTransport()
				.IsTransactional(true)
				.PurgeOnStartup(false)
				.UnicastBus()
				.SendOnly();
		}

		protected virtual void RegisterDocumentStore(IWindsorContainer container)
		{
			var store = new DocumentStore
				{
					ConnectionStringName = Settings.RavenDbConnectionStringName,
					ResourceManagerId = Guid.NewGuid()
				};

			store.Initialize();

			string domainDbName = ConfigurationManager.AppSettings["RavenDbName"];
			store.DatabaseCommands.EnsureDatabaseExists(domainDbName);

			string nServiceBusDbName = ConfigurationManager.AppSettings["NServiceBus.Persistence.RavenDbName"];
			store.DatabaseCommands.EnsureDatabaseExists(nServiceBusDbName);

			container.Register(Component.For<IDocumentStore>().Instance(store));
		}
	}
}