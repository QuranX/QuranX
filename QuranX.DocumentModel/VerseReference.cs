using System;

namespace QuranX.DocumentModel
{
	public class VerseReference :
		IComparable,
		IComparable<VerseReference>
	{
		public readonly int Chapter;
		public readonly int Verse;

		public VerseReference(int chapter, int verse)
		{
			this.Chapter = chapter;
			this.Verse = verse;

			QuranStructure.ValidateChapterAndVerse(
					chapter: chapter,
					verse: verse
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
			return
				this.Chapter == other.Chapter
				&& this.Verse == other.Verse;
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
			if (this.Chapter < other.Chapter)
				return -1;
			if (this.Chapter > other.Chapter)
				return 1;
			if (this.Verse < other.Verse)
				return -1;
			if (this.Verse > other.Verse)
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
