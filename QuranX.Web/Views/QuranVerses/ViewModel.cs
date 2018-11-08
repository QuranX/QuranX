using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Web.Models;

namespace QuranX.Web.Views.QuranVerses
{
	public class ViewModel
	{
		public readonly IEnumerable<ChapterAndVerseSelection> DisplayVerses;
		public readonly SelectChapterAndVerse SelectChapterAndVerse;
		public readonly bool AutoScrollToSelectedVerse;
		public readonly IEnumerable<Translator> Translators;

		public ViewModel(
			IEnumerable<ChapterAndVerseSelection> displayVerses,
			SelectChapterAndVerse selectChapterAndVerse,
			bool autoScrollToSelectedVerse)
		{
			DisplayVerses = displayVerses;
			SelectChapterAndVerse = selectChapterAndVerse;
			AutoScrollToSelectedVerse = autoScrollToSelectedVerse;
			Translators = DisplayVerses
				.SelectMany(x => x.Verses)
				.FirstOrDefault()
				?.VerseTexts
				?.Select(x => new Translator(x.TranslatorCode, x.TranslatorName))
				?? Array.Empty<Translator>();
		}
	}
}