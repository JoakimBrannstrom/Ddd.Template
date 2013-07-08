using System.Web.Mvc;

namespace Ddd.Template.Web.Controllers
{
	public sealed class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult About()
		{
			return View();
		}

		public ActionResult Contact()
		{
			return View();
		}

		public ActionResult ClientInformation()
		{
			return View();
		}
	}
}
