using System;
using System.Configuration;
using System.IO;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Ddd.Template.Contracts;
using Ddd.Template.Contrib;
using Ddd.Template.Domain.CommandHandlers;
using NServiceBus;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Extensions;
using log4net.Config;

namespace Ddd.Template.Server.Bootstrap
{
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization
	{
		public void Init()
		{
			var container = BootstrapContainer();

			SetNServiceBusLoggingLibrary();

			var configuration = GetConfigurationInstance();

			AddUnbotrusiveConventions(configuration);

			ConfigureNServiceBus(configuration, container);

			RegisterDocumentStore(container);

			RunInstallers(container);
		}

		protected virtual void RunInstallers(IWindsorContainer container)
		{
			container.Install(FromAssembly.This());
		}

		protected virtual IWindsorContainer BootstrapContainer()
		{
			var container = new WindsorContainer();

			// http://stw.castleproject.org/Windsor.Windsor-Tutorial-Part-Five-Adding-logging-support.ashx?HL=ilogger
			container
				.AddFacility<LoggingFacility>(f => f.UseLog4Net(Settings.Log4NetConfigurationFilename));

			var logger = container.Resolve<ILogger>();
			logger.Debug("Installing Ddd.Template.Server components");

			return container;
		}

		private static void SetNServiceBusLoggingLibrary()
		{
			// http://nservicebus.com/Logging.aspx#customized
			SetLoggingLibrary.Log4Net(() => XmlConfigurator
				                                .Configure(new FileInfo(Settings.Log4NetConfigurationFilename)));
		}

		private Configure GetConfigurationInstance()
		{
			return Configure
					.With(new[] { typeof(CommandHandlerBase<>).Assembly, typeof(ConsoleInteraction).Assembly });
		}

		private static void AddUnbotrusiveConventions(Configure configuration)
		{
			var commandTypeDefinition = MessageConfigurator.GetMessageTypeDefinition(MessageType.Command);
			var eventTypeDefinition = MessageConfigurator.GetMessageTypeDefinition(MessageType.Event);

			configuration
				.DefiningCommandsAs(commandTypeDefinition)
				.DefiningEventsAs(eventTypeDefinition)
				;//.DefiningEncryptedPropertiesAs(p => p.Name.StartsWith("Encrypted"));
		}

		protected virtual void ConfigureNServiceBus(Configure configuration, IWindsorContainer container)
		{
			Func<string> connectionStringFactory = GetConnectionStringFactory("NServiceBus.Persistence");

			configuration
				.DefineEndpointName("Ddd.Template.Domain")
				.CastleWindsorBuilder(container)
				.JsonSerializer()
				.MsmqTransport()
				.IsTransactional(true)
				.PurgeOnStartup(false)
				.RavenPersistence(connectionStringFactory, "Ddd.Template.NServiceBus.domain")
				.RunTimeoutManager()
				.UseRavenTimeoutPersister()
				.RavenSubscriptionStorage()
				.UnicastBus()
				.CreateBus()
				.Start();
		}

		protected virtual Func<string> GetConnectionStringFactory(string name)
		{
			return () => ConfigurationManager.ConnectionStrings[name].ConnectionString;
		}

		protected virtual void RegisterDocumentStore(IWindsorContainer container)
		{
			var store = CreateDocumentStore();

			store.Initialize();

			var domainDbName = ConfigurationManager.AppSettings["RavenDbName"];
			store.EnsureDatabaseExists(domainDbName);

			var nServiceBusDbName = ConfigurationManager.AppSettings["NServiceBus.Persistence.RavenDbName"];
			store.EnsureDatabaseExists(nServiceBusDbName);

			container.Register(Component.For<IDocumentStore>().Instance(store));
		}

		protected virtual IDocumentStore CreateDocumentStore()
		{
			return new DocumentStore
			{
				ConnectionStringName = Settings.RavenDbConnectionStringName,
				ResourceManagerId = Guid.NewGuid()
			};
		}
	}
}
