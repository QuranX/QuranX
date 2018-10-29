using System.Collections.Generic;
using System.Linq;
using NLog;
using QuranX.DataMigration.Services;
using QuranX.DocumentModel;
using QuranX.Persistence.Services.Repositories;
using XmlDocument = QuranX.DocumentModel.Document;
using AnalysisWordViewModel = QuranX.Persistence.Models.VerseAnalysisWord;
using AnalysisWordPartViewModel = QuranX.Persistence.Models.VerseAnalysisWordPart;

namespace QuranX.DataMigration.Migrators
{
	public interface ICorpusMigrator
	{
		void Migrate();
	}

	public class CorpusMigrator : ICorpusMigrator
	{
		private readonly XmlDocument XmlDocument;
		private readonly ILogger Logger;
		private readonly IVerseAnalysisWordWriteRepository AnalysisWordWriteRepository;

		public CorpusMigrator(
			IXmlDocumentProvider xmlDocumentProvider,
			ILogger logger,
			IVerseAnalysisWordWriteRepository analysisWordWriteRepository)
		{
			XmlDocument = xmlDocumentProvider.Document;
			Logger = logger;
			AnalysisWordWriteRepository = analysisWordWriteRepository;
		}

		public void Migrate()
		{
			IEnumerable<CorpusVerse> verses = XmlDocument.CorpusDocument.Verses
				.OrderBy(x => x.Reference.Chapter)
				.ThenBy(x => x.Reference.Verse)
				.ThenBy(x => x.Index);
			foreach (CorpusVerse corpusVerse in verses)
			{
				Logger.Debug("Corpus " + corpusVerse.Reference);
				WriteVerse(corpusVerse);
			}
		}

		private void WriteVerse(CorpusVerse corpusVerse)
		{
			string[] arabicWords = corpusVerse.ArabicText.Split(' ');
			foreach (var word in corpusVerse.Words)
			{
				var analysisWordParts = word.Parts
					.Select(x => new AnalysisWordPartViewModel(
						root: x.Root,
						type: x.TypeCode,
						description: x.TypeDescription,
						decorators: x.Decorators.Select(d => d.Trim())));
				var analysisWord = new AnalysisWordViewModel(
					chapterNumber: corpusVerse.Reference.Chapter,
					verseNumber: corpusVerse.Reference.Verse,
					wordNumber: word.Index,
					arabic: arabicWords[word.Index - 1],
					english: word.English,
					buckwalter: word.Buckwalter,
					wordParts: analysisWordParts);
				AnalysisWordWriteRepository.Write(analysisWord);
			}
		}
	}
}
