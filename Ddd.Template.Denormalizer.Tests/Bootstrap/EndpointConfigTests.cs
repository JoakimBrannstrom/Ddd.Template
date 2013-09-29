using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Ddd.Template.Denormalizer.Bootstrap;
using Ddd.Template.Contrib;
using NServiceBus.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NServiceBus;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ddd.Template.Denormalizer.Tests.Bootstrap
{
	[TestClass]
	public class EndpointConfigTests : EndpointConfig
	{
		Mock<IDocumentStore> _fakeStore = new Mock<IDocumentStore>();

		[TestInitialize]
		public void Setup()
		{
			ConfigurationManager.AppSettings["RavenDbName"] = "dbName";
			ConfigurationManager.AppSettings["NServiceBus.Persistence.RavenDbName"] = "Persistence.dbName";

			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			if (config.Sections.Get("MessageForwardingInCaseOfFaultConfig") == null)
				config.Sections.Add("MessageForwardingInCaseOfFaultConfig", new MessageForwardingInCaseOfFaultConfig());
			var section = (MessageForwardingInCaseOfFaultConfig)config.GetSection("MessageForwardingInCaseOfFaultConfig");
			section.ErrorQueue = "error";
			config.Save(ConfigurationSaveMode.Modified);
			ConfigurationManager.RefreshSection("MessageForwardingInCaseOfFaultConfig");

			RavenExtensions.DatabaseExistsFactory = (StoredEntityEventArgs, name) => { };
		}

		[TestMethod]
		public void GivenNoInitialState_WhenEndpointConfigIsInitialized_ThenDocumentStoreShouldBeInitialized()
		{
			_fakeStore
				.Setup(s => s.Initialize())
				.Verifiable("Store was not initialized!");

			Init();

			_fakeStore.Verify();
		}

		protected override Castle.Windsor.IWindsorContainer BootstrapContainer()
		{
			return new WindsorContainer();
		}

		protected override Func<string> GetConnectionStringFactory(string name)
		{
			return () => "Url=http://localhost:8080/;Database=Ddd.Template.NServiceBus.domain";
		}

		protected override IDocumentStore CreateDocumentStore()
		{
			return _fakeStore.Object;
		}

		protected override void RunInstallers(IWindsorContainer container)
		{
		}
	}
}
