using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Ddd.Template.Domain;
using Ddd.Template.Domain.Aggregates;

namespace Ddd.Template.Server.Bootstrap
{
	public class RepositoryInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container
				.Register(Component
							.For<IRepository<Visitor>>()
							.ImplementedBy(typeof(Repository<Visitor>))
							.LifeStyle.Singleton);
		}
	}
}
