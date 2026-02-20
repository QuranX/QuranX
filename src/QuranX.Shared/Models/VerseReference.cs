using System;

namespace QuranX.Shared.Models;

public record VerseReference : IComparable<VerseReference>
{
    public int Chapter { get; }
    public int Verse { get; }

    public VerseReference(int chapter, int verse)
    {
        Chapter = chapter;
        Verse = verse;
    }

    public static int GetIndexValue(int chapterNumber, int verseNumber) =>
        chapterNumber * 1000 + verseNumber;

    public int ToIndexValue() => GetIndexValue(Chapter, Verse);

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

    public string ToDisplayText() => string.Format("{0}.{1}", Chapter, Verse);
    public override string ToString() => ToDisplayText();

    int IComparable<VerseReference>.CompareTo(VerseReference other)
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
}
