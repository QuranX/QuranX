using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;

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
			ILogger logger = CreateLogger();
			SharedData.Initialize(logger);
			SearchEngine.Initialize();
		}

		private ILogger CreateLogger()
		{
			var config = new NLog.Config.LoggingConfiguration();
			var logConsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
			config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
			LogManager.Configuration = config;
			ILogger logger = NLog.LogManager.GetCurrentClassLogger();
			return logger;
		}

		public override string GetVaryByCustomString(HttpContext context, string arg)
		{
			if (arg.Equals("translations", StringComparison.InvariantCultureIgnoreCase))
				return context.Request.Cookies["translations"]?.Value;
			return base.GetVaryByCustomString(context, arg);
		}
	}
}