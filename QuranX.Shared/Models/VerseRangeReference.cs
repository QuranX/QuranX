using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace QuranX.Shared.Models
{
	public class VerseRangeReference :
		IComparable,
		IComparable<VerseRangeReference>,
		IEnumerable<VerseReference>
	{
		public readonly int Chapter;
		public readonly int FirstVerse;
		public readonly int LastVerse;

		public VerseRangeReference(int chapter, int firstVerse, int lastVerse)
		{
			QuranStructure.EnsureChapterAndVerseAreValid(chapter, firstVerse);
			QuranStructure.EnsureChapterAndVerseAreValid(chapter, lastVerse);

			Chapter = chapter;
			FirstVerse = firstVerse;
			LastVerse = lastVerse;
		}

		public IEnumerable<VerseReference> ToVerseReferences()
		{
			var result = new List<VerseReference>();
			for (int index = FirstVerse; index <= LastVerse; index++)
				result.Add(new VerseReference(chapter: Chapter, verse: index));
			return result;
		}

		public static VerseRangeReference Parse(string source)
		{
			string[] chapterVerseParts = source.Split('.');
			string[] verseRangeParts = chapterVerseParts[1].Split('-');
			int chapter = int.Parse(chapterVerseParts[0]);
			int firstVerse = int.Parse(verseRangeParts[0]);
			int lastVerse = firstVerse;
			if (verseRangeParts.Length > 1)
				lastVerse = int.Parse(verseRangeParts[1]);
			return new VerseRangeReference(
					chapter: chapter,
					firstVerse: firstVerse,
					lastVerse: lastVerse
				);
		}

		public static VerseRangeReference ParseXml(XElement parentNode)
		{
			int chapter = int.Parse(parentNode.Element("chapter").Value);
			int firstVerse = int.Parse(parentNode.Element("firstVerse").Value);
			int lastVerse = int.Parse(parentNode.Element("lastVerse").Value);
			return new VerseRangeReference(
					chapter: chapter,
					firstVerse: firstVerse,
					lastVerse: lastVerse
				);
		}

		public bool Includes(int chapter, int verse)
		{
			return chapter == Chapter
				&& verse >= FirstVerse
				&& verse <= LastVerse;
		}

		public override string ToString()
		{
			if (LastVerse == FirstVerse)
				return string.Format("{0}.{1}", Chapter, FirstVerse);
			return string.Format("{0}.{1}-{2}", Chapter, FirstVerse, LastVerse);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is VerseRangeReference))
				return false;

			var other = (VerseRangeReference)obj;
			return
				Chapter == other.Chapter
				&& FirstVerse == other.FirstVerse
				&& LastVerse == other.LastVerse;
		}

		public static bool operator ==(VerseRangeReference left, VerseRangeReference right)
		{
			return (left.Equals(right));
		}

		public static bool operator !=(VerseRangeReference left, VerseRangeReference right)
		{
			return (!left.Equals(right));
		}

		public int CompareTo(VerseRangeReference other)
		{
			if (Chapter < other.Chapter)
				return -1;
			if (Chapter > other.Chapter)
				return 1;
			if (FirstVerse < other.FirstVerse)
				return -1;
			if (FirstVerse > other.FirstVerse)
				return 1;
			if (LastVerse < other.LastVerse)
				return -1;
			if (LastVerse > other.LastVerse)
				return 1;
			return 0;
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is VerseRangeReference))
				throw new ArgumentException();
			return CompareTo((VerseRangeReference)obj);
		}


		IEnumerator<VerseReference> IEnumerable<VerseReference>.GetEnumerator()
		{
			for (int verseIndex = FirstVerse; verseIndex <= LastVerse; verseIndex++)
			{
				yield return new VerseReference(chapter: Chapter, verse: verseIndex);
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<VerseReference>).GetEnumerator();
		}
	}
}
