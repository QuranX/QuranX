using System.IO;
using System.Reflection;
using NLog;
using QuranX.DataMigration.Migrators;
using QuranX.Persistence.Services;
using Unity;

namespace QuranX.DataMigration.Services
{
	public static class Registration
	{
		public static void Register(IUnityContainer container)
		{
			RegisterXmlSettings(container);
			RegisterWebSettings(container);
			RegisterLogger(container);
			container.RegisterSingleton<IXmlDocumentProvider, XmlDocumentProvider>();
			container.RegisterSingleton<IDataMigrator, DataMigrator>();
			container.RegisterSingleton<IQuranMigrator, QuranMigrator>();
			container.RegisterSingleton<ICommentaryMigrator, CommentaryMigrator>();
			container.RegisterSingleton<IHadithMigrator, HadithMigrator>();
			container.RegisterSingleton<ICorpusMigrator, CorpusMigrator>();
		}

		private static void RegisterXmlSettings(IUnityContainer container)
		{
			string xmlDataPath = Path.Combine(GetAppDirectory(), "App_Data");

			var xmlSettings = new XmlSettings(xmlDataPath);
			container.RegisterInstance<IXmlSettings>(xmlSettings);
		}

		private static void RegisterWebSettings(IUnityContainer container)
		{
			string webDataPath = Path.Combine(GetAppDirectory(), "..", "QuranX.Web", "App_Data");

			var webSettings = new Settings(webDataPath);
			container.RegisterInstance<ISettings>(webSettings);
		}

		private static void RegisterLogger(IUnityContainer container)
		{
			var config = new NLog.Config.LoggingConfiguration();
			var logConsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
			config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
			LogManager.Configuration = config;
			ILogger logger = NLog.LogManager.GetCurrentClassLogger();
			container.RegisterInstance<ILogger>(logger);
		}

		private static string GetAppDirectory()
		{
			string xmlDataPath = Assembly.GetExecutingAssembly().Location;
			string appName = Path.GetFileNameWithoutExtension(xmlDataPath).ToLowerInvariant();
			int index = xmlDataPath.ToLowerInvariant().IndexOf(appName);
			xmlDataPath = xmlDataPath.Substring(0, index + appName.Length);
			return xmlDataPath;
		}
	}
}
