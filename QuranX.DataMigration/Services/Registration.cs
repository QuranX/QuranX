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
			RegisterConfiguration(container);
			RegisterWebSettings(container);
			RegisterLogger(container);
			container.RegisterSingleton<IXmlDocumentProvider, XmlDocumentProvider>();
			container.RegisterSingleton<IDataMigrator, DataMigrator>();
			container.RegisterSingleton<IQuranMigrator, QuranMigrator>();
			container.RegisterSingleton<ICommentaryMigrator, CommentaryMigrator>();
			container.RegisterSingleton<IHadithMigrator, HadithMigrator>();
			container.RegisterSingleton<ICorpusMigrator, CorpusMigrator>();
			container.RegisterSingleton<ILanesLexiconMigrator, LanesLexiconMigrator>();
		}

		private static void RegisterConfiguration(IUnityContainer container)
		{
			string appDataPath = Path.Combine(GetAppDirectory(), "App_Data");
			string dictionariesDataPath = Path.Combine(appDataPath, "Dictionaries");

			var configuration = new Configuration(appDataPath);
			container.RegisterInstance<IConfiguration>(configuration);
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
