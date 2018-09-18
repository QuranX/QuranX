using System;

namespace QuranX.Shared.Models
{
	public class VerseReference :
		IComparable,
		IComparable<VerseReference>
	{
		public int Chapter { get; set; }
		public int Verse { get; set; }

		public VerseReference() { }

		public VerseReference(int chapter, int verse)
		{
			Chapter = chapter;
			Verse = verse;

			QuranStructure.EnsureChapterAndVerseAreValid(
					chapterNumber: chapter,
					verseNumber: verse
				);
		}

		public static VerseReference Parse(string source)
		{
			string[] chapterVerseParts = source.Split('.');
			int chapter = int.Parse(chapterVerseParts[0]);
			int verse = int.Parse(chapterVerseParts[1]);
			return new VerseReference(
					chapter: chapter,
					verse: verse
				);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", Chapter, Verse);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is VerseReference))
				return false;

			var other = (VerseReference)obj;
			return Chapter == other.Chapter && Verse == other.Verse;
		}

		public static bool operator ==(VerseReference left, VerseReference right)
		{
			return (left.Equals(right));
		}

		public static bool operator !=(VerseReference left, VerseReference right)
		{
			return (!left.Equals(right));
		}

		public int CompareTo(VerseReference other)
		{
			if (Chapter < other.Chapter)
				return -1;
			if (Chapter > other.Chapter)
				return 1;
			if (Verse < other.Verse)
				return -1;
			if (Verse > other.Verse)
				return 1;
			return 0;
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is VerseReference))
				throw new ArgumentException();
			return CompareTo((VerseReference)obj);
		}
	}
}
