using System.Web.Mvc;

namespace Ddd.Template.Web.Scaffolding.Configuration
{
	public static class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
