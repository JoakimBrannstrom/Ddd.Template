using System.Web.Mvc;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ddd.Template.Web.Controllers;

namespace Ddd.Template.Web.Scaffolding.Installers
{
	public sealed class ControllersInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container
				.Register(Classes
							.FromThisAssembly()
							.BasedOn<IController>()
							.If(Component.IsInSameNamespaceAs<HomeController>())
							.If(t => t.Name.EndsWith("Controller"))
							.Configure(config =>
							{
								config
									/*
									.Interceptors(
										new InterceptorReference(typeof(AuthorizeOwnerInterceptor))
									).First
									*/
									.LifeStyle.Is(LifestyleType.Transient);
							}));
		}
	}
}