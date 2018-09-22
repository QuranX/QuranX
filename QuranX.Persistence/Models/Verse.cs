using System;
using System.Collections.Generic;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Models
{
	public class Verse : IComparable, IComparable<Verse>
	{
		public int Id { get; set; }
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

			Id = VerseReference.GetIndexValue(chapterNumber, verseNumber);
			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			RootWordCount = rootWordCount;
			HadithCount = hadithCount;
			CommentaryCount = commentaryCount;
			VerseTexts = verseTexts ?? throw new ArgumentNullException(nameof(verseTexts));
		}

		public override int GetHashCode()
		{
			return $"{ChapterNumber}.{VerseNumber}".GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Verse))
				return false;

			var other = (Verse)obj;
			return
				ChapterNumber == other.ChapterNumber
				&& VerseNumber == other.VerseNumber;
		}

		public static bool operator ==(Verse left, Verse right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Verse left, Verse right)
		{
			return !left.Equals(right);
		}

		public int CompareTo(Verse other)
		{
			if (ChapterNumber < other.ChapterNumber)
				return -1;
			if (ChapterNumber > other.ChapterNumber)
				return 1;
			if (VerseNumber < other.VerseNumber)
				return -1;
			if (VerseNumber > other.VerseNumber)
				return 1;
			return 0;
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is Verse))
				throw new ArgumentException();
			return CompareTo((Verse)obj);
		}

	}
}
