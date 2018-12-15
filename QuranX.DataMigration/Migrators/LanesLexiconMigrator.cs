using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NLog;
using QuranX.DataMigration.Services;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;

namespace QuranX.DataMigration.Migrators
{
	public interface ILanesLexiconMigrator
	{
		void Migrate();
	}

	public class LanesLexiconMigrator : ILanesLexiconMigrator
	{
		private readonly Regex NewLineRegex;
		private readonly IConfiguration Configuration;
		private readonly IDictionaryWriteRepository DictionaryWriteRepository;
		private readonly IDictionaryEntryWriteRepository DictionaryEntryWriteRepository;
		private readonly ILogger Logger;

		public LanesLexiconMigrator(
			IConfiguration configuration,
			IDictionaryWriteRepository dictionaryWriteRepository,
			IDictionaryEntryWriteRepository dictionaryEntryWriteRepository,
			ILogger logger)
		{
			Configuration = configuration;
			DictionaryWriteRepository = dictionaryWriteRepository;
			DictionaryEntryWriteRepository = dictionaryEntryWriteRepository;
			Logger = logger;
			NewLineRegex = new Regex(@"(\w*\<br.?\>\w*)+");
		}

		public void Migrate()
		{
			string jsonFilePath = Path.Combine(Configuration.DictionariesDirectoryPath, "Lane.json");
			string json = File.ReadAllText(jsonFilePath);
			var jsonDoc = JValue.Parse(json);
			var firstProperty = (JProperty)jsonDoc.First;
			var currentRoot = (JProperty)firstProperty.Value.First;
			int count = 0;
			while (currentRoot != null)
			{
				string root = ArabicHelper.Standardize(currentRoot.Name);
				string rootLetterNames = ArabicHelper.ArabicToLetterNames(root);
				if (count++ % 100 == 0)
					Logger.Debug($"Lane's lexicon {rootLetterNames}");
				string rootHtml = currentRoot.Value.ToString();
				string html = currentRoot.Value.ToString().Replace("\r", "").Replace("\n", "");
				string[] htmlLines = NewLineRegex.Replace(html, "\r").Split('\r');
				var dictionaryEntry = new DictionaryEntry(
					dictionaryCode: "Lane",
					code: root,
					html: htmlLines);
				DictionaryEntryWriteRepository.Write(dictionaryEntry);

				currentRoot = (JProperty)currentRoot.Next;
			}
		}

		void WriteDictionary()
		{
			string copyrightFilePath = Path.Combine(Configuration.DictionariesDirectoryPath, "Lane-Copyright.txt");
			string copyright = File.ReadAllText(copyrightFilePath);
			var dictionary = new Dictionary(code: "Lane", name: "Lane's Lexicon", copyright: copyright);
			DictionaryWriteRepository.Write(dictionary);
		}
	}
}
