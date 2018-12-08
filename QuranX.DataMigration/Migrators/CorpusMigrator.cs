using System.Collections.Generic;
using System.Linq;
using NLog;
using QuranX.DataMigration.Services;
using QuranX.DocumentModel;
using QuranX.Persistence.Services.Repositories;
using XmlDocument = QuranX.DocumentModel.Document;
using VerseAnalysisVM = QuranX.Persistence.Models.VerseAnalysis;
using AnalysisWordVM = QuranX.Persistence.Models.VerseAnalysisWord;
using AnalysisWordPartVM = QuranX.Persistence.Models.VerseAnalysisWordPart;

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
		private readonly IVerseAnalysisWriteRepository VerseAnalysisWriteRepository;

		public CorpusMigrator(
			IXmlDocumentProvider xmlDocumentProvider,
			ILogger logger,
			IVerseAnalysisWriteRepository verseAnalysisWriteRepository)
		{
			XmlDocument = xmlDocumentProvider.Document;
			Logger = logger;
			VerseAnalysisWriteRepository = verseAnalysisWriteRepository;
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
			var words = new List<AnalysisWordVM>();
			foreach (var word in corpusVerse.Words)
			{
				var analysisWordParts = word.Parts
					.Select(x => new AnalysisWordPartVM(
						root: x.Root,
						type: x.TypeCode,
						subType: x.SubType,
						description: x.TypeDescription,
						decorators: x.Decorators.Select(d => d.Trim())));
				var analysisWord = new AnalysisWordVM(
					wordNumber: word.Index,
					english: word.English,
					buckwalter: word.Buckwalter,
					wordParts: analysisWordParts);
				words.Add(analysisWord);
			}
			var verseAnalysis = new VerseAnalysisVM(
				chapterNumber: corpusVerse.Reference.Chapter,
				verseNumber: corpusVerse.Reference.Verse,
				words: words);
			VerseAnalysisWriteRepository.Write(verseAnalysis);
		}
	}
}
