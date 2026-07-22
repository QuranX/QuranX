using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;

namespace QuranX.Shared.Models;

// TODO: Ensure chapter + verse are valid
public class VerseRangeReference : IComparable, IComparable<VerseRangeReference>, IGetDisplayText
{
    [Description("Quran chapter in the range 1 to 114.")]
    public int Chapter { get; set; }

    [Description("Number of the first verse of the range within the chapter.")]
    public int FirstVerse { get; set; }

    [Description($"Number of the last verse of the range within the chapter. If 0 then it is assumed to be the same as {nameof(FirstVerse)}.")]
    public int LastVerse { set => field = value; get => field == 0 ? FirstVerse : field; }

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
        if (!TryParseComponents(source, out int chapter, out int firstVerse, out int lastVerse))
            throw new FormatException(
                $"'{source}' is not a valid verse range reference " +
                "(expected 'chapter.verse' or 'chapter.firstVerse-lastVerse').");
        return new VerseRangeReference(
                chapter: chapter,
                firstVerse: firstVerse,
                lastVerse: lastVerse
            );
    }

    /// <summary>
    /// Parses a verse range reference and validates it against the bounds of the Quran
    /// (chapter 1-114, verse within the chapter's verse count) and rejects inverted ranges.
    /// Returns <c>false</c> for null, malformed, or out-of-range input instead of throwing.
    /// </summary>
    public static bool TryParse(string? source, [NotNullWhen(true)] out VerseRangeReference? result)
    {
        result = null;
        if (!TryParseComponents(source, out int chapter, out int firstVerse, out int lastVerse))
            return false;
        if (!QuranStructure.TryValidateChapterAndVerse(chapter, firstVerse))
            return false;
        if (!QuranStructure.TryValidateChapterAndVerse(chapter, lastVerse))
            return false;
        if (lastVerse < firstVerse)
            return false;
        result = new VerseRangeReference(chapter, firstVerse, lastVerse);
        return true;
    }

    private static bool TryParseComponents(string? source, out int chapter, out int firstVerse, out int lastVerse)
    {
        chapter = 0;
        firstVerse = 0;
        lastVerse = 0;
        if (string.IsNullOrWhiteSpace(source))
            return false;
        string[] chapterVerseParts = source.Split('.');
        if (chapterVerseParts.Length != 2)
            return false;
        string[] verseRangeParts = chapterVerseParts[1].Split('-');
        if (verseRangeParts.Length > 2)
            return false;
        if (!int.TryParse(chapterVerseParts[0], out chapter))
            return false;
        if (!int.TryParse(verseRangeParts[0], out firstVerse))
            return false;
        lastVerse = firstVerse;
        if (verseRangeParts.Length > 1)
        {
            if (!int.TryParse(verseRangeParts[1], out lastVerse))
                return false;
            if (lastVerse == 0)
                lastVerse = firstVerse;
        }
        return true;
    }

    public static VerseRangeReference ParseXml(XElement parentNode)
    {
        int chapter = int.Parse(parentNode.Element("chapter")!.Value);
        int firstVerse = int.Parse(parentNode.Element("firstVerse")!.Value);
        int lastVerse = int.Parse(parentNode.Element("lastVerse")!.Value);
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

    public override string ToString() => GetDisplayText();

    public string GetDisplayText()
    {
        if (LastVerse == FirstVerse)
            return string.Format("{0}.{1}", Chapter, FirstVerse);
        return string.Format("{0}.{1}-{2}", Chapter, FirstVerse, LastVerse);
    }

    public override int GetHashCode() => ToString().GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is not VerseRangeReference other)
            return false;

        return
            Chapter == other.Chapter
            && FirstVerse == other.FirstVerse
            && LastVerse == other.LastVerse;
    }

    public static bool operator ==(VerseRangeReference? left, VerseRangeReference? right)
    {
        if (left is null && right is null) return true;
        if (left is null != right is null) return false;
        return left!.Equals(right);
    }

    public static bool operator !=(VerseRangeReference left, VerseRangeReference right)
    {
        return !(left == right);
    }

    public int CompareTo(VerseRangeReference? other)
    {
        if (other is null) return 1;
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

    int IComparable.CompareTo(object? obj)
    {
        if (obj is not VerseRangeReference other)
            throw new ArgumentException();
        return CompareTo(other);
    }

    public static IEnumerable<VerseRangeReference> Simplify(
        IEnumerable<VerseRangeReference> verseReferences,
        IEnumerable<VerseRangeReference>? excludedVerseReferences = null)
    {
        HashSet<(int Chapter, int Verse)> convertToHashSet(IEnumerable<VerseRangeReference>? verseReferences)
        {
            if (verseReferences is null) return [];

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

        VerseRangeReference? currentRef = null;
        foreach (var item in ordered)
        {
            if (item.Chapter != currentChapter || item.Verse != previousVerse + 1)
            {
                if (currentRef is not null)
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
        if (currentRef is not null)
        {
            currentRef = new VerseRangeReference(currentRef.Chapter, currentRef.FirstVerse, previousVerse);
            result.Add(currentRef);
        }
        return result;
    }
}
