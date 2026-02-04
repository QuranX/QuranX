using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace QuranX.Shared.Models
{
	public class VerseRangeReference : IComparable, IComparable<VerseRangeReference>
	{
		public int Chapter { get; set; }
		public int FirstVerse { get; set; }
		public int LastVerse { get; set; }
		public bool IsMultipleVerses() => FirstVerse != LastVerse;

		public VerseRangeReference() { }

		public VerseRangeReference(int chapter, int firstVerse, int lastVerse)
		{
			Chapter = chapter;
			FirstVerse = firstVerse;
			LastVerse = lastVerse;
		}

		public static int GetIndexValue(int chapterNumber, int firstVerseNumber, int lastVerseNumber)
			=> (chapterNumber * 1000000) + (firstVerseNumber * 1000) + lastVerseNumber;
		public int ToIndexValue() => GetIndexValue(Chapter, FirstVerse, LastVerse);

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
			return (!left?.Equals(right) == true);
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

		public static IEnumerable<VerseRangeReference> Simplify(IEnumerable<VerseRangeReference> verseReferences)
		{
			if (!verseReferences.Any()) return Enumerable.Empty<VerseRangeReference>();

			var hashSet = new HashSet<(int Chapter, int Verse)>();
			foreach (VerseRangeReference reference in verseReferences)
			{
				for (int verse = reference.FirstVerse; verse <= reference.LastVerse; verse++)
					hashSet.Add((reference.Chapter, verse));
			}

			var ordered = hashSet.OrderBy(x => x.Chapter).ThenBy(x => x.Verse);

			var result = new List<VerseRangeReference>();
			int currentChapter = ordered.First().Chapter;
			int currentFirstVerse = ordered.First().Verse;
			int previousVerse = currentFirstVerse;

			VerseRangeReference currentRef = null;
			foreach (var item in ordered)
			{
				if (item.Chapter != currentChapter || item.Verse != previousVerse + 1)
				{
					if (currentRef != null)
					{
						currentRef = new VerseRangeReference(currentChapter, currentFirstVerse, previousVerse);
						result.Add(currentRef);
					}

					currentChapter = item.Chapter;
					currentFirstVerse = item.Verse;
					currentRef = new VerseRangeReference(currentChapter, currentFirstVerse, currentFirstVerse);
				}
				previousVerse = item.Verse;
			}
			if (currentRef != null)
			{
				currentRef = new VerseRangeReference(currentRef.Chapter, currentRef.FirstVerse, previousVerse);
				result.Add(currentRef);
			}
			return result;
		}
	}
}
