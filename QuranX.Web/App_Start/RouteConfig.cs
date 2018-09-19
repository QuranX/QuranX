using System.Web.Mvc;
using System.Web.Routing;
using QuranX.Web.Controllers;

namespace QuranX.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "QuranVerses",
				url: "x",///{*verses}",
				defaults: new { Controller = "QuranVerses", Action = "Index", Verses="1.1-2,2.1-200,3.1.200,4.1-100,5.1-100" },
				constraints: new
				{
					//verses = @"^(\d+\.\d+(-\d+)?)(,(\d+\.\d+(-\d+)?))*$"
				}
			);
			routes.MapRoute(
				name: "QuranChapters",
				url: "",
				defaults: new { Controller = "QuranChapters", Action = nameof(QuranChaptersController.Index) }
			);
		}
	}
}
