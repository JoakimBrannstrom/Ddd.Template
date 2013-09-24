using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database;
using System.Web.Mvc;
using Ddd.Template.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Web.Tests.Controllers
{
	[TestClass]
	public sealed class HomeControllerTest
	{
		IDocumentStore _store;
		IDocumentSession _session;
		HomeController _controller;

		[TestInitialize]
		public void Setup()
		{
			_store = new EmbeddableDocumentStore
			{
				RunInMemory = true
			};

			_store.Initialize();

			_session = _store.OpenSession();
			_controller = new HomeController(_session);
		}

		[TestCleanup]
		public void Cleanup()
		{
			_session.Dispose();
			_store.Dispose();
		}

		[TestMethod]
		public void Index()
		{
			// Arrange

			// Act
			var result = _controller.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(null, result.ViewBag.Message);
		}

		[TestMethod]
		public void About()
		{
			// Arrange

			// Act
			var result = _controller.About() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Contact()
		{
			// Arrange

			// Act
			var result = _controller.Contact() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
