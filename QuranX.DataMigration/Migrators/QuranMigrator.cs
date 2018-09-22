using QuranX.DocumentModel;
using QuranX.Persistence.Services.Repositories;
using System.Collections.Generic;
using System.Linq;
using QuranX.DataMigration.Services;
using VerseViewModel = QuranX.Persistence.Models.Verse;
using VerseTextViewModel = QuranX.Persistence.Models.VerseText;
using XmlDocument = QuranX.DocumentModel.Document;
using NLog;

namespace QuranX.DataMigration.Migrators
{
	public interface IQuranMigrator
	{
		void Migrate();
	}

	public class QuranMigrator : IQuranMigrator
	{
		private readonly XmlDocument XmlDocument;
		private readonly ILogger Logger;
		private readonly IVerseWriterRepository VerseWriterRepository;

		public QuranMigrator(
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			IVerseWriterRepository verseWriterRepository)
		{
			Logger = logger;
			XmlDocument = xmlDocumentProvider.Document;
			VerseWriterRepository = verseWriterRepository;
		}

		public void Migrate()
		{
			WriteVerses();
		}

		private void WriteVerses()
		{
			foreach (Chapter chapter in XmlDocument.QuranDocument.Chapters)
			{
				Logger.Debug($"Writing chapter {chapter.Index}");
				foreach (Verse verse in chapter.Verses)
				{
					WriteVerse(chapter, verse);
				}
			}
		}

		private void WriteVerse(Chapter chapter, Verse verse)
		{
			var verseTexts = new List<VerseTextViewModel>();
			verseTexts.Add(new VerseTextViewModel("AR", "Arabic", verse.ArabicText));
			foreach (VerseTranslation translation in verse.Translations.OrderBy(x => x.TranslatorName))
			{
				verseTexts.Add(new VerseTextViewModel(translation.TranslatorCode, translation.TranslatorCode, translation.Text));
			}

			var verseViewModel = new VerseViewModel(
				chapterNumber: chapter.Index,
				verseNumber: verse.Index,
				rootWordCount: XmlDocument.CorpusDocument[chapter.Index, verse.Index].Words.Count(),
				hadithCount: XmlDocument.HadithDocument.GetHadithsForVerse(chapter.Index, verse.Index).Count(),
				commentaryCount: XmlDocument.TafsirDocument.GetCommentaries(chapter.Index, verse.Index).Count(),
				verseTexts: verseTexts);
			VerseWriterRepository.Write(verseViewModel);
		}
	}
}
