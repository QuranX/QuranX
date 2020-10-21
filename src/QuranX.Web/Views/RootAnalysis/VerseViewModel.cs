using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.RootAnalysis
{
	public class VerseViewModel
	{
		public readonly int ChapterNumber;
		public readonly int VerseNumber;
		public readonly VerseAnalysisWord SelectedWord;
		public readonly VerseAnalysisWordPart SelectedWordPart;
		public readonly IReadOnlyList<VerseAnalysisWord> Words;

		public VerseViewModel(
			int chapterNumber,
			int verseNumber,
			VerseAnalysisWord selectedWord,
			VerseAnalysisWordPart selectedWordPart,
			IEnumerable<VerseAnalysisWord> words)
		{
			if (words == null)
				throw new ArgumentNullException(nameof(words));

			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			SelectedWord = selectedWord;
			SelectedWordPart = selectedWordPart;
			Words = words.ToList().AsReadOnly();
		}
	}
}