using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using QuranX.DataMigration.Migrators;
using QuranX.Persistence.Services;

namespace QuranX.DataMigration.Services
{
	public static class Registration
	{
		public static void Register(IServiceCollection services)
		{
			RegisterConfiguration(services);
			RegisterWebSettings(services);
			RegisterLogger(services);
			services.AddSingleton<IXmlDocumentProvider, XmlDocumentProvider>();
			services.AddSingleton<IDataMigrator, DataMigrator>();
			services.AddSingleton<IQuranMigrator, QuranMigrator>();
			services.AddSingleton<ICommentaryMigrator, CommentaryMigrator>();
			services.AddSingleton<IHadithMigrator, HadithMigrator>();
			services.AddSingleton<ICorpusMigrator, CorpusMigrator>();
			services.AddSingleton<IDictionariesMigrator, DictionariesMigrator>();
			services.AddSingleton<ILisaanDictionaryMigrator, LisaanDictionaryMigrator>();
		}

		private static void RegisterConfiguration(IServiceCollection services)
		{
			string appDataPath = Path.Combine(GetAppDirectory(), "App_Data");

			var configuration = new Configuration(appDataPath);
			services.AddSingleton<IConfiguration>(configuration);
		}

		private static void RegisterWebSettings(IServiceCollection services)
		{
			string webDataPath = Path.Combine(GetAppDirectory(), "..", "QuranX.Web", "App_Data");

			var webSettings = new Settings(webDataPath);
			services.AddSingleton<ISettings>(webSettings);
		}

		private static void RegisterLogger(IServiceCollection services)
		{
			var config = new NLog.Config.LoggingConfiguration();
			var logConsole = new NLog.Targets.ColoredConsoleTarget("logconsole");
			config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
			LogManager.Configuration = config;
			ILogger logger = NLog.LogManager.GetCurrentClassLogger();
			services.AddSingleton<ILogger>(logger);
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
