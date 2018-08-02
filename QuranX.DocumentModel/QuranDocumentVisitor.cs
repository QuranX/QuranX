using System.Collections.Generic;

namespace QuranX.DocumentModel
{
	public class QuranDocumentVisitor
	{

		protected virtual void VisitQuran(QuranDocument document)
		{
			VisitChapters(document.Chapters);
		}

		protected virtual void VisitChapters(IEnumerable<Chapter> chapters)
		{
			foreach (Chapter chapter in chapters)
				VisitChapter(chapter);
		}

		protected virtual void VisitChapter(Chapter chapter)
		{
			VisitVerses(chapter.Verses);
		}

		protected virtual void VisitVerses(IEnumerable<Verse> verses)
		{
			foreach (Verse verse in verses)
				VisitVerse(verse);
		}

		protected virtual void VisitVerse(Verse verse)
		{
			VisitVerseTranslations(verse.Translations);
		}

		protected virtual void VisitVerseTranslations(IEnumerable<VerseTranslation> translations)
		{
			foreach (VerseTranslation translation in translations)
				VisitVerseTranslation(translation);
		}

		void VisitVerseTranslation(VerseTranslation translation)
		{
		}

	}
}
