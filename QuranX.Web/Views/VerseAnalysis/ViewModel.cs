using System;
using System.Collections.Generic;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Views.VerseAnalysis
{
	public class ViewModel
	{
		public readonly Chapter Chapter;
		public readonly int VerseNumber;
		public IEnumerable<VerseAnalysisWord> Words;
		public readonly SelectChapterAndVerse SelectChapterAndVerse;

		public ViewModel(
			Chapter chapter,
			int verseNumber,
			IEnumerable<VerseAnalysisWord> words,
			SelectChapterAndVerse selectChapterAndVerse)
		{
			Chapter = chapter;
			VerseNumber = verseNumber;
			Words = words ?? throw new ArgumentNullException(nameof(words));
			SelectChapterAndVerse = selectChapterAndVerse;
		}
	}
}