using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace QuranX.Shared.Models;

// TODO: Ensure chapter + verse are valid
public record VerseRangeReference : IComparable<VerseRangeReference>
{
    public int Chapter { get; }
    public int FirstVerse { get; }
    public int LastVerse { get; }
    public bool IsMultipleVerses => FirstVerse != LastVerse;

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
        for (int index = FirstVerse; index <= LastVerse; index++)
            yield return new VerseReference(chapter: Chapter, verse: index);
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

    public bool Includes(int chapter, int verse) =>
        chapter == Chapter && verse >= FirstVerse && verse <= LastVerse;

    public string ToDisplayText() =>
        LastVerse == FirstVerse
        ? string.Format("{0}.{1}", Chapter, FirstVerse)
        : string.Format("{0}.{1}-{2}", Chapter, FirstVerse, LastVerse);

    public override string ToString() => ToDisplayText();

    int IComparable<VerseRangeReference>.CompareTo(VerseRangeReference other)
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

    public static IEnumerable<VerseRangeReference> Simplify(
        IEnumerable<VerseRangeReference> verseReferences,
        IEnumerable<VerseRangeReference> excludedVerseReferences = null)
    {
        HashSet<(int Chapter, int Verse)> convertToHashSet(IEnumerable<VerseRangeReference> verseReferences)
        {
            if (verseReferences == null) return [];

            var result = new HashSet<(int Chapter, int Verse)>();
            foreach (VerseRangeReference reference in verseReferences)
            {
                for (int verse = reference.FirstVerse; verse <= reference.LastVerse; verse++)
                    result.Add((reference.Chapter, verse));
            }
            return result;
        }

        if (!verseReferences.Any()) return Enumerable.Empty<VerseRangeReference>();

        var verses = convertToHashSet(verseReferences).Except(convertToHashSet(excludedVerseReferences));
        var ordered =  verses.OrderBy(x => x.Chapter).ThenBy(x => x.Verse);
        if (!ordered.Any()) return [];

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
