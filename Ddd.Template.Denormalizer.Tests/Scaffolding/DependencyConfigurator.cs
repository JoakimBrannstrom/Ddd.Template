using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ddd.Template.Contracts.Events;
using Moq;
using NServiceBus;
using Raven.Client;
using Raven.Client.Embedded;

namespace Ddd.Template.Denormalizer.Tests.Scaffolding
{
	public class DependencyConfigurator<TEvent, TEventHandler> : IDisposable
		where TEvent : Event
		where TEventHandler : IHandleMessages<TEvent>
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

		public TType GetDependency<TType>() where TType : class
		{
			return (TType)_dependencies[typeof(TType)];
		}

		public IHandleMessages<TEvent> BuildEventHandler()
		{
			_dependencies = new Dictionary<Type, object>();

			var constructorInfo = typeof(TEventHandler).GetConstructors().First();

			var parameters = InitializeFakes(constructorInfo).ToArray();

			return (IHandleMessages<TEvent>)constructorInfo.Invoke(parameters);
		}

		private IEnumerable<object> InitializeFakes(ConstructorInfo constructorInfo)
		{
			foreach (var parameter in constructorInfo.GetParameters())
			{
				if (parameter.ParameterType == typeof(IDocumentStore))
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

					_dependencies.Add(typeof(IDocumentStore), store);
					yield return store;
					continue;
				}

				var mock = CreateMock(parameter.ParameterType);
				_dependencies.Add(parameter.ParameterType, mock);
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
