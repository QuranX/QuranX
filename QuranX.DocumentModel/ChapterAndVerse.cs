using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuranX.DocumentModel
{
	public class ChapterAndVerse : IComparable
	{
		public Chapter Chapter { get; private set; }
		public Verse Verse { get; private set; }

		public ChapterAndVerse(Chapter chapter, Verse verse)
		{
			this.Chapter = chapter;
			this.Verse = verse;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ChapterAndVerse))
				return false;

			var other = (ChapterAndVerse)obj;
			return
				this.Chapter.Index == other.Chapter.Index
				&& this.Verse.Index == other.Verse.Index;
		}

		public static bool operator ==(ChapterAndVerse left, ChapterAndVerse right)
		{
			return (left.Equals(right));
		}

		public static bool operator !=(ChapterAndVerse left, ChapterAndVerse right)
		{
			return (!left.Equals(right));
		}

		public int CompareTo(ChapterAndVerse other)
		{
			if (this.Chapter.Index < other.Chapter.Index)
				return -1;
			if (this.Chapter.Index > other.Chapter.Index)
				return 1;
			if (this.Verse.Index < other.Verse.Index)
				return -1;
			if (this.Verse.Index > other.Verse.Index)
				return 1;
			return 0;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return Chapter.GetHashCode() + Verse.GetHashCode();
			}
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is ChapterAndVerse))
				throw new ArgumentException();
			return CompareTo((ChapterAndVerse)obj);
		}
	}
}
