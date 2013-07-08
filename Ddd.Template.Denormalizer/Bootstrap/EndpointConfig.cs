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

namespace Ddd.Template.Denormalizer.Bootstrap
{
	public sealed class EndpointConfig : IConfigureThisEndpoint, AsA_Client, IWantCustomInitialization
	{
		public void Init()
		{
			var container = BootstrapContainer();

			SetNServiceBusLoggingLibrary();

			var configuration = GetConfigurationInstance();

			AddUnbotrusiveConventions(configuration);

			ConfigureNServiceBus(configuration, container);

			RegisterDocumentStore(container);

			container.Install(FromAssembly.This());
		}

		private IWindsorContainer BootstrapContainer()
		{
			var container = new WindsorContainer();

			// http://stw.castleproject.org/Windsor.Windsor-Tutorial-Part-Five-Adding-logging-support.ashx?HL=ilogger
			container
				.AddFacility<LoggingFacility>(f => f.UseLog4Net(Settings.Log4NetConfigurationFilename));

			var logger = container.Resolve<ILogger>();
			logger.Debug("Installing Ddd.Template.Denormalizer components");

			return container;
		}

		private void SetNServiceBusLoggingLibrary()
		{
			// http://nservicebus.com/Logging.aspx#customized
			SetLoggingLibrary.Log4Net(() => log4net
											.Config
											.XmlConfigurator
											.Configure(new FileInfo(Settings.Log4NetConfigurationFilename)));
		}

		private Configure GetConfigurationInstance()
		{
			return Configure.With();
		}

		private static void AddUnbotrusiveConventions(Configure configuration)
		{
			var commandTypeDefinition = MessageConfigurator.GetMessageTypeDefinition(MessageType.Command);
			var eventTypeDefinition = MessageConfigurator.GetMessageTypeDefinition(MessageType.Event);

			configuration
					.DefiningCommandsAs(commandTypeDefinition)
					.DefiningEventsAs(eventTypeDefinition)
					; //.DefiningEncryptedPropertiesAs(p => p.Name.StartsWith("Encrypted"));
		}

		private void ConfigureNServiceBus(Configure configuration, IWindsorContainer container)
		{
			var nServiceBusDbName = ConfigurationManager.AppSettings["NServiceBus.Persistence.RavenDbName"];

			configuration
				.DefineEndpointName("Ddd.Template.Denormalizer")
				.CastleWindsorBuilder(container)
				.RavenPersistence("NServiceBus.Persistence", nServiceBusDbName)
				.RunTimeoutManager()
					.UseRavenTimeoutPersister()
				.JsonSerializer()
				.MsmqTransport()
					.IsTransactional(true)
					.PurgeOnStartup(false)
				.UnicastBus()
				.CreateBus()
				.Start();
		}

		private void RegisterDocumentStore(IWindsorContainer container)
		{
			var store = new DocumentStore
			{
				ConnectionStringName = Settings.RavenDbConnectionStringName,
				ResourceManagerId = Guid.NewGuid()
			};

			store.Initialize();

			var projectionsDbName = ConfigurationManager.AppSettings["RavenDbName"];
			store.DatabaseCommands.EnsureDatabaseExists(projectionsDbName);

			var nServiceBusDbName = ConfigurationManager.AppSettings["NServiceBus.Persistence.RavenDbName"];
			store.DatabaseCommands.EnsureDatabaseExists(nServiceBusDbName);

			container.Register(Component.For<IDocumentStore>().Instance(store));
		}
	}
}
