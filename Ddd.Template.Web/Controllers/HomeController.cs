using Ddd.Template.Web.Scaffolding.RavenIndexes;
using Raven.Client;
using System.Linq;
using System.Web.Mvc;

namespace Ddd.Template.Web.Controllers
{
	public sealed class HomeController : Controller
	{
		IDocumentSession _documentSession;

		public HomeController(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
		}

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

		public ActionResult VisitorInformation()
		{
			var visitors = _documentSession
							.Query<VisitorCount>(typeof(AllVisitorsIndex).Name, true)
							.ToArray();

			return View(visitors);
		}
	}
}
