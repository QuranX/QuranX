namespace QuranX.Shared.Models;

public class ChapterData
{
    public int ChapterNumber { get; }
    public string ArabicName { get; }
    public string EnglishName { get; }
    public int NumberOfVerses { get; }
    public int RevelationOrder { get; }

    public ChapterData(
        int chapterNumber,
        string arabicName,
        string englishName,
        int numberOfVerses,
        int revelationOrder)
    {
        ChapterNumber = chapterNumber;
        ArabicName = arabicName;
        EnglishName = englishName;
        NumberOfVerses = numberOfVerses;
        RevelationOrder = revelationOrder;
    }
}
