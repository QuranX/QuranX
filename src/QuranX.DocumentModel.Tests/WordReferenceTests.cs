namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class WordReferenceTests
{
    private static WordReference Make(
        int chapter = 2,
        int verse = 3,
        int word = 4,
        int wordPart = 5) =>
        new(
            root: "r-h-m",
            chapterIndex: chapter,
            verseIndex: verse,
            wordIndex: word,
            wordPartIndex: wordPart,
            wordPartType: "N",
            wordPartTypeDescription: "Noun",
            buckwalterText: "rHm",
            englishText: "mercy");

    [Fact]
    public void GetDisplayText_FormatsChapterColonVerseDotWord()
    {
        Assert.Equal("2:3.4", Make(2, 3, 4).GetDisplayText());
    }

    [Fact]
    public void ToString_DelegatesToGetDisplayText()
    {
        Assert.Equal("2:3.4", Make(2, 3, 4).ToString());
    }

    [Fact]
    public void LocationKey_Includes4PartCompositeKey()
    {
        Assert.Equal("2:3:4:5", Make(2, 3, 4, 5).LocationKey);
    }

    [Fact]
    public void GetLocationKey_StaticHelper_FormatsSameAsInstance()
    {
        Assert.Equal("2:3:4:5", WordReference.GetLocationKey(2, 3, 4, 5));
    }
}
