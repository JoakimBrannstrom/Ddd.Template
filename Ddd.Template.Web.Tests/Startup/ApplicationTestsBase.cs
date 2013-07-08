using System.Web;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NServiceBus;

namespace Ddd.Template.Web.Tests.Startup
{
	[TestClass]
	public abstract class ApplicationTestsBase : MvcApplication
	{
		protected readonly Mock<IBus> FakeBus = new Mock<IBus>();
		readonly Mock<IWindsorContainer> _fakeContainer = new Mock<IWindsorContainer>();

		readonly Mock<HttpApplicationStateBase> _fakeApplication = new Mock<HttpApplicationStateBase>(MockBehavior.Loose);
		readonly Mock<HttpRequestBase> _fakeRequest = new Mock<HttpRequestBase>();
		protected readonly Mock<HttpSessionStateBase> FakeSession = new Mock<HttpSessionStateBase>();

		[TestInitialize]
		public void Setup()
		{
			_fakeContainer
				.Setup(c => c.Resolve<IBus>())
				.Returns(FakeBus.Object);

			_fakeApplication.DefaultValue = DefaultValue.Mock;

			_fakeRequest.Setup(r => r.UserLanguages).Returns(new[] { "sv" });
			_fakeRequest.Setup(r => r.UserAgent).Returns("Unit test");
			_fakeRequest.Setup(r => r.UserHostAddress).Returns("localtest");
			_fakeRequest.Setup(r => r.UserHostName).Returns("Local Test");
			_fakeRequest.Setup(r => r.Browser.Platform).Returns("test platform");

			Container = _fakeContainer.Object;
		}

		protected override HttpApplicationStateBase GetHttpApplication()
		{
			return _fakeApplication.Object;
		}

		protected override HttpRequestBase GetHttpRequest()
		{
			return _fakeRequest.Object;
		}

		protected override HttpSessionStateBase GetSession()
		{
			return FakeSession.Object;
		}
	}
}