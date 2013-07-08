using System.IO;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ddd.Template.Contracts;
using NServiceBus;

namespace Ddd.Template.Web.Scaffolding.Installers
{
	public sealed class NServiceBusInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			SetLoggingLibrary.Log4Net(() => log4net
												.Config
												.XmlConfigurator
												.Configure(new FileInfo(Settings.Log4NetConfigurationFilename)));

			var configure = Configure.With();

			AddUnbotrusiveConventions(configure);

			configure
				.CastleWindsorBuilder(container)
				.JsonSerializer()
				.MsmqTransport()
				.IsTransactional(false)
				.PurgeOnStartup(false)
				.UnicastBus()
				.SendOnly();
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
	}
}