using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Models
{
	public class Verse : IComparable, IComparable<Verse>
	{
		public int Id { get; }
		public int ChapterNumber { get; }
		public int VerseNumber { get; }
		public int RootWordCount { get; }
		public int HadithCount { get; }
		public int CommentaryCount { get; }
		public IReadOnlyList<VerseText> VerseTexts { get; }

		public Verse(
			int chapterNumber,
			int verseNumber,
			int rootWordCount, 
			int hadithCount,
			int commentaryCount,
			IEnumerable<VerseText> verseTexts)
		{
			QuranStructure.EnsureChapterAndVerseAreValid(chapterNumber, verseNumber);

			Id = VerseReference.GetIndexValue(chapterNumber, verseNumber);
			ChapterNumber = chapterNumber;
			VerseNumber = verseNumber;
			RootWordCount = rootWordCount;
			HadithCount = hadithCount;
			CommentaryCount = commentaryCount;
			VerseTexts = verseTexts.ToList().AsReadOnly();
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
