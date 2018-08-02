using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.DocumentModel
{
	public class QuranDocument
	{
		readonly Dictionary<int, Chapter> _Chapters;
        string[] AllTranslationCodes;

		public QuranDocument()
		{
			this._Chapters = new Dictionary<int, Chapter>();
		}

		public IEnumerable<Chapter> Chapters
		{
			get
			{
				return _Chapters
					.Select(x => x.Value)
					.OrderBy(x => x.Index);
			}
		}

		public Chapter this[int chapterIndex]
		{
			get
			{
				return _Chapters[chapterIndex];
			}
		}

		public Verse this[int chapterIndex, int verseIndex]
		{
			get
			{
				var chapter = this[chapterIndex];
				return chapter[verseIndex];
			}
		}

		public int ChapterCount
		{
			get { return _Chapters.Count; }
		}

		public void AddChapter(Chapter chapter)
		{
			_Chapters.Add(chapter.Index, chapter);
		}

		public IEnumerable<ChapterAndVerse> GetVersesInRange(VerseRangeReference verseRangeReference)
		{
			var result = new List<ChapterAndVerse>();
			foreach (var verseReference in verseRangeReference.ToVerseReferences())
			{
				var chapter = this[verseReference.Chapter];
				var verse = chapter[verseReference.Verse];
				result.Add(new ChapterAndVerse(chapter: chapter, verse: verse));
			}
			return result;
		}

		public IEnumerable<ChapterAndVerse> GetVersesInRange(IEnumerable<VerseRangeReference> verseRangeReferences)
		{
			var result = new List<ChapterAndVerse>();
			foreach (var verseRangeReference in verseRangeReferences)
				result.AddRange(GetVersesInRange(verseRangeReference));
			return result
				.Distinct();
		}

        public string[] GetDefaultTranslatorCodes()
        {
            return new string[] { "Pickthall", "YusufAli", "SahihIntl", "Arabic" };
        }

        public string[] GetAllTranslatorCodes()
        {
            if (AllTranslationCodes == null)
            {
                var codes = this[1, 1].Translations.Select(x => x.TranslatorCode).ToList();
                codes.Add("Arabic");
                AllTranslationCodes = codes.ToArray();
            }
            return AllTranslationCodes;
        }


	}
}
