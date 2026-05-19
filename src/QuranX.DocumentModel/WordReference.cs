using QuranX.Shared;

namespace QuranX.DocumentModel;

public class WordReference : IGetDisplayText
{
    public readonly string Root;
    public readonly int ChapterIndex;
    public readonly int VerseIndex;
    public readonly int WordIndex;
    public readonly int WordPartIndex;
    public readonly string WordType;
    public readonly string WordTypeDescription;
    public readonly string BuckwalterText;
    public readonly string EnglishText;

    public WordReference(
        string root,
        WordPartLocation location,
        WordTypeInfo wordType,
        string buckwalterText,
        string englishText)
    {
        Root = root;
        ChapterIndex = location.ChapterIndex;
        VerseIndex = location.VerseIndex;
        WordIndex = location.WordIndex;
        WordPartIndex = location.WordPartIndex;
        WordType = wordType.Type;
        WordTypeDescription = wordType.Description;
        BuckwalterText = buckwalterText;
        EnglishText = englishText;
    }

    public override string ToString() => GetDisplayText();

    public string GetDisplayText()
    {
        return string.Format(
                "{0}:{1}.{2}",
                ChapterIndex,
                VerseIndex,
                WordIndex
            );
    }

    public static string GetLocationKey(
        int chapterIndex,
        int verseIndex,
        int wordIndex,
        int wordPartIndex)
    {
        return string.Format("{0}:{1}:{2}:{3}",
                chapterIndex,
                verseIndex,
                wordIndex,
                wordPartIndex
            );
    }

    public string LocationKey
    {
        get
        {
            return GetLocationKey(
                    chapterIndex: ChapterIndex,
                    verseIndex: VerseIndex,
                    wordIndex: WordIndex,
                    wordPartIndex: WordPartIndex
                );
        }
    }
}
