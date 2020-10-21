using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class VerseAnalysis
	{
		public int ChapterNumber { get; }
		public int VerseNumber { get; }
		public IReadOnlyList<VerseAnalysisWord> Words { get; }

		public VerseAnalysis(
			int chapterNumber,
			int verseNumber,
			IReadOnlyList<VerseAnalysisWord> words)
		{
			if (words == null)
				throw new ArgumentNullException(nameof(words));

			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			Words = words.ToList().AsReadOnly();
		}
	}
}
