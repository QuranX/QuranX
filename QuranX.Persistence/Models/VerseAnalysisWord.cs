using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class VerseAnalysisWord
	{
		public int ChapterNumber { get; }
		public int VerseNumber { get; }
		public int WordNumber { get; }
		public string Arabic { get; }
		public string English { get; }
		public string Buckwalter { get; }
		public IReadOnlyCollection<VerseAnalysisWordPart> WordParts { get; }

		public VerseAnalysisWord(
			int chapterNumber,
			int verseNumber,
			int wordNumber,
			string arabic,
			string english,
			string buckwalter,
			IEnumerable<VerseAnalysisWordPart> wordParts)
		{
			if (wordParts == null)
				throw new ArgumentNullException(nameof(wordParts));

			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			WordNumber = wordNumber;
			Arabic = arabic;
			English = english;
			Buckwalter = buckwalter;
			WordParts = wordParts.ToList().AsReadOnly();
		}
	}
}
