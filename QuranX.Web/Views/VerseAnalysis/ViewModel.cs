using System;
using System.Collections.Generic;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Web.Views.VerseAnalysis
{
	public class ViewModel
	{
		public readonly Chapter Chapter;
		public readonly int VerseNumber;
		public IEnumerable<VerseAnalysisWord> Words;

		public ViewModel(
			Chapter chapter,
			int verseNumber,
			IEnumerable<VerseAnalysisWord> words)
		{
			if (words == null)
				throw new ArgumentNullException(nameof(words));

			Chapter = chapter;
			VerseNumber = verseNumber;
			Words = words;
		}
	}
}