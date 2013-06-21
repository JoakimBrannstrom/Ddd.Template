using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using DocumentStation.Contracts.Commands;
using DocumentStation.Web.App_Start;
using DocumentStation.Web.Scaffolding;
using NServiceBus;

namespace DocumentStation.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected static IWindsorContainer Container;

		protected void Application_Start()
		{
			BootstrapContainer();

			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();

			Container
				.Resolve<IBus>()
				.Send(new AddDocument { Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString() });
		}

		protected void Application_End()
		{
			Container.Dispose();
		}

		private static void BootstrapContainer()
		{
			Container = new WindsorContainer().Install(FromAssembly.This());

			// Let windsor handle all our IoC, things'll just work! :]
			//DependencyResolver.SetResolver(new WindsorDependepcyResolver(_container));
			var controllerFactory = new WindsorControllerFactory(Container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}
	}
}
	