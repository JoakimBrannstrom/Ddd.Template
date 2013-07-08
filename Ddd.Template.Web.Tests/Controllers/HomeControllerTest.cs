using System.Web.Mvc;
using Ddd.Template.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ddd.Template.Web.Tests.Controllers
{
	[TestClass]
	public sealed class HomeControllerTest
	{
		[TestMethod]
		public void Index()
		{
			// Arrange
			var controller = new HomeController();

			// Act
			var result = controller.Index() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(null, result.ViewBag.Message);
		}

		[TestMethod]
		public void About()
		{
			// Arrange
			var controller = new HomeController();

			// Act
			var result = controller.About() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod]
		public void Contact()
		{
			// Arrange
			var controller = new HomeController();

			// Act
			var result = controller.Contact() as ViewResult;

			// Assert
			Assert.IsNotNull(result);
		}
	}
}
