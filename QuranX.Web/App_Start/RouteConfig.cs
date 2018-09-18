using System.Web.Mvc;
using System.Web.Routing;

namespace QuranX.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "QuranChapters",
				url: "",
				defaults: new { Controller = "QuranChapters", Action = "Index" }
			);
		}
	}
}
