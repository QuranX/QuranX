using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using QuranX.DataMigration.Services;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;

namespace QuranX.DataMigration.Migrators
{
	public interface ILisaanDictionaryMigrator
	{
		void Migrate();
	}

	public class LisaanDictionaryMigrator : ILisaanDictionaryMigrator
	{
		private readonly Regex NewLineRegex;
		private readonly Regex HeaderRegex;
		private readonly IConfiguration Configuration;
		private readonly IDictionaryWriteRepository DictionaryWriteRepository;
		private readonly IDictionaryEntryWriteRepository DictionaryEntryWriteRepository;
		private readonly ILogger Logger;

		public LisaanDictionaryMigrator(
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
			HeaderRegex = new Regex(@"\<h\d\>.*?\</h\d\>");
		}

		public void Migrate()
		{
			Migrate("Lane");
			Migrate("Salmone");
		}

		public void Migrate(string code)
		{
			string jsonFilePath = Path.Combine(Configuration.DictionariesDirectoryPath, $"{code}.json");
			JsonDictionary jsonDictionary;
			using (StreamReader sr = File.OpenText(jsonFilePath))
			using (var jsonReader = new JsonTextReader(sr))
			{
				var serializer = new JsonSerializer();
				jsonDictionary = serializer.Deserialize<JsonDictionary>(jsonReader);
			}
			WriteDictionary(code, jsonDictionary.Name);
			int index = 0;
			foreach(var entry in jsonDictionary.Entries)
			{
				index++;
				string root = ArabicHelper.Substitute(entry.Name);
				string rootLetterNames = ArabicHelper.ArabicToLetterNames(root);
				string html = entry.Text.Replace("\r", "").Replace("\n", "");
				html = HeaderRegex.Replace(html, "");
				string[] htmlLines = NewLineRegex.Replace(html, "\r").Split('\r');
				var dictionaryEntry = new DictionaryEntry(
					dictionaryCode: code,
					word: root,
					entryIndex: index,
					html: htmlLines);
				DictionaryEntryWriteRepository.Write(dictionaryEntry);

				if (index % 100 == 0)
					Logger.Debug($"{code} {rootLetterNames}");
			}
		}

		void WriteDictionary(string code, string name)
		{
			name = name.Split('<')[0].Trim();
			string copyrightFilePath = Path.Combine(Configuration.DictionariesDirectoryPath, $"{code}-Copyright.txt");
			string copyright = File.ReadAllText(copyrightFilePath);
			var dictionary = new Dictionary(code: code, name: name, copyright: copyright);
			DictionaryWriteRepository.Write(dictionary);
		}

		private class JsonDictionary
		{
			public string Name { get; set; }
			public JsonDictionaryEntry[] Entries { get; set; }
		}

		private class JsonDictionaryEntry
		{
			public string Name { get; set; }
			public string Text { get; set; }
		}
	}
}
