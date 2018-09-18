using System;
using System.Collections.Generic;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Models
{
	public class Verse
	{
		public int ChapterNumber { get; set; }
		public int VerseNumber { get; set; }
		public int RootWordCount { get; set; }
		public int HadithCount { get; set; }
		public int CommentaryCount { get; set; }
		public IReadOnlyList<VerseText> VerseTexts { get; set; }

		public Verse() { }

		public Verse(int chapterNumber, int verseNumber, int rootWordCount, int hadithCount, int commentaryCount, IReadOnlyList<VerseText> verseTexts)
		{
			QuranStructure.EnsureChapterAndVerseAreValid(chapterNumber, verseNumber);

			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			RootWordCount = rootWordCount;
			HadithCount = hadithCount;
			CommentaryCount = commentaryCount;
			VerseTexts = verseTexts ?? throw new ArgumentNullException(nameof(verseTexts));
		}
	}
}
