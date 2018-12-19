using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
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
		private readonly Regex ArabicRegex;
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
			ArabicRegex = new Regex(@"(\p{IsArabic}+)");
		}

		public void Migrate()
		{
			Migrate("Lane");
			Migrate("Salmone");
		}

		public void Migrate(string code)
		{
			string jsonFilePath = Path.Combine(Configuration.DictionariesDirectoryPath, $"{code}");
			var jsonDictionary = ReadJsonObject<JsonDictionary>(jsonFilePath + ".json");
			var jsonDictionaryMeta = ReadJsonObject<DictionaryMeta>(jsonFilePath + "-meta.json");
			WriteDictionary(code, jsonDictionary.Name, jsonDictionaryMeta.Copyright);
			int index = 0;
			foreach (var entry in jsonDictionary.Entries)
			{
				index++;
				string root = ArabicHelper.Substitute(entry.Name);
				string rootLetterNames = ArabicHelper.ArabicToLetterNames(root);
				string html = HeaderRegex.Replace(entry.Text, "");
				if (jsonDictionaryMeta.RemoveNewLines)
				{
					html = html.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
					html = NewLineRegex.Replace(html, " ");
				}
				html = ArabicRegex.Replace(html, m => $"<span class=\"arabic\">{m.Value}</span>");
				string[] htmlLines = html.Split('\r');
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

		T ReadJsonObject<T>(string jsonFilePath)
		{
			using (StreamReader sr = File.OpenText(jsonFilePath))
			using (var jsonReader = new JsonTextReader(sr))
			{
				var serializer = new JsonSerializer();
				return serializer.Deserialize<T>(jsonReader);
			}
		}

		void WriteDictionary(string code, string name, string copyright)
		{
			name = name.Split('<')[0].Trim();
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

		private class DictionaryMeta
		{
			public string Copyright { get; set; }
			public bool RemoveNewLines { get; set; }
		}
	}
}
