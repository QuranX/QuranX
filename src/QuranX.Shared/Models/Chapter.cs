using System;

namespace QuranX.Shared.Models;

public record Chapter : IComparable<Chapter>
{
    public int ChapterNumber { get; }
    public string ArabicName { get; }
    public string EnglishName { get; }
    public string Period { get; }
    public int NumberOfVerses { get; }
    public int RevelationOrder { get; }

    public Chapter(
        int chapterNumber,
        string arabicName,
        string englishName,
        string period,
        int numberOfVerses,
        int revelationOrder)
    {
        ChapterNumber = chapterNumber;
        ArabicName = arabicName;
        EnglishName = englishName;
        Period = period;
        NumberOfVerses = numberOfVerses;
        RevelationOrder = revelationOrder;
    }

    int IComparable<Chapter>.CompareTo(Chapter other) => ChapterNumber.CompareTo(other.ChapterNumber);
}
