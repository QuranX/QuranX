using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace QuranX
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			SharedData.Initialize();
			SearchEngine.Initialize();
		}

		public override string GetVaryByCustomString(HttpContext context, string arg)
		{
			if (arg.Equals("translations", StringComparison.InvariantCultureIgnoreCase))
				return context.Request.Cookies["translations"]?.Value;
			return base.GetVaryByCustomString(context, arg);
		}
	}
}