using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ddd.Template.Contracts;
using Ddd.Template.Contracts.Commands;
using Ddd.Template.Domain;
using Ddd.Template.Server.CommitDispatchers;
using EventStore;
using EventStore.Dispatcher;
using Moq;
using NServiceBus;
using Raven.Client;
using Raven.Client.Embedded;

namespace Ddd.Template.Server.Tests.Scaffolding
{
	public class DependencyConfigurator<TAggregateRoot, TCommand, TCommandHandler> : IDisposable
		where TAggregateRoot : AggregateRoot, new()
		where TCommand : Command
		where TCommandHandler : IHandleMessages<TCommand>
	{
		private Dictionary<Type, object> _dependencies;

		public void Dispose()
		{
			foreach (var dependency in _dependencies)
			{
				var disposable = dependency.Value as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
		}

		public void SetupDependencies()
		{
			_dependencies = new Dictionary<Type, object>();

			var fakeBus = new Mock<IBus>();
			_dependencies.Add(typeof(IBus), fakeBus);

			var domainViewDispatcher = InitializeDomainViewDispatcher();

			var busPublisher = new Mock<NServiceBusPublisher>(fakeBus.Object, domainViewDispatcher);
			_dependencies.Add(typeof(NServiceBusPublisher), busPublisher);

			var eventStore = WireupEventStore(busPublisher.Object);
			_dependencies.Add(typeof(IStoreEvents), eventStore);
		}

		private RavenDomainViewDispatcher InitializeDomainViewDispatcher()
		{
			var store = new EmbeddableDocumentStore
			{
				Configuration =
				{
					RunInMemory = true,
					RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true
				}
			};

			store.Initialize();
			var domainViewDispatcher = new RavenDomainViewDispatcher(store);
			_dependencies.Add(typeof(IDocumentStore), store);
			_dependencies.Add(typeof(RavenDomainViewDispatcher), domainViewDispatcher);
			return domainViewDispatcher;
		}

		private static IStoreEvents WireupEventStore(IDispatchCommits bus)
		{
			return Wireup
					.Init()
					.LogToOutputWindow()
					.UsingInMemoryPersistence()
					.InitializeStorageEngine()
					.UsingSynchronousDispatchScheduler(bus)
					.Build();
		}

		public Mock<TType> GetMockedDependency<TType>() where TType : class
		{
			return (Mock<TType>)_dependencies[typeof(TType)];
		}

		public TType GetDependency<TType>() where TType : class
		{
			return (TType)_dependencies[typeof(TType)];
		}

		public IHandleMessages<TCommand> BuildCommandHandler()
		{
			var constructorInfo = typeof(TCommandHandler).GetConstructors().First();

			var parameters = InitializeConstructorParameters(constructorInfo).ToArray();

			return (IHandleMessages<TCommand>)constructorInfo.Invoke(parameters);
		}

		private IEnumerable<object> InitializeConstructorParameters(ConstructorInfo constructorInfo)
		{
			foreach (var parameter in constructorInfo.GetParameters())
			{
				if (parameter.ParameterType == typeof(IRepository<TAggregateRoot>))
				{
					var fakeRepository = new Mock<Repository<TAggregateRoot>>(GetDependency<IStoreEvents>());

					fakeRepository.CallBase = true;	// http://code.google.com/p/moq/wiki/QuickStart#Customizing_Mock_Behavior
					yield return fakeRepository.Object;
					continue;
				}

				if (parameter.ParameterType == typeof(IDocumentStore))
				{
					var store = GetDependency<IDocumentStore>();
					yield return store;
					continue;
				}

				yield return ((Mock)CreateMock(parameter.ParameterType)).Object;
			}
		}

		private static object CreateMock(Type type)
		{
			var constructorInfo = typeof(Mock<>).MakeGenericType(type).GetConstructors().First();
			return constructorInfo.Invoke(new object[] { });
		}
	}
}
