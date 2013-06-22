using System.Web.Mvc;

namespace DocumentStation.Web.Scaffolding.Configuration
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}