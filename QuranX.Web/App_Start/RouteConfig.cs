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
				name: "Commentators",
				url: "Tafsirs",
				defaults: new { Controller = "Commentators", Action = "Index" });

			routes.MapRoute(
				name: "QuranVerses",
				url: "{*Verses}",
				defaults: new { Controller = "QuranVerses", Action = "Index" },
				constraints: new
				{
					Verses = @"^(\d+\.\d+(-\d+)?)(,(\d+\.\d+(-\d+)?))*$"
				});

			routes.MapRoute(
				name: "QuranChapters",
				url: "",
				defaults: new { Controller = "QuranChapters", Action = nameof(QuranChaptersController.Index) });
		}
	}
}
