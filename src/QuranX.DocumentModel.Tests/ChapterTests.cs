namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class ChapterTests
{
    [Fact]
    public void Constructor_StoresFields()
    {
        var chapter = new Chapter(1, "Al-Faatiha", "الفاتحة");
        Assert.Equal(1, chapter.Number);
        Assert.Equal("Al-Faatiha", chapter.EnglishName);
        Assert.Equal("الفاتحة", chapter.ArabicName);
        Assert.Equal(0, chapter.VerseCount);
    }

    [Fact]
    public void FullName_CombinesEnglishAndArabic()
    {
        Assert.Equal(
            "Al-Faatiha - الفاتحة",
            new Chapter(1, "Al-Faatiha", "الفاتحة").FullName);
    }

    [Fact]
    public void AddVerse_IncrementsCountAndAllowsLookup()
    {
        var chapter = new Chapter(2, "Al-Baqara", "البقرة");
        var verse = new Verse(1, "بسم");
        chapter.AddVerse(verse);
        Assert.Equal(1, chapter.VerseCount);
        Assert.Same(verse, chapter[1]);
    }

    [Fact]
    public void Verses_AreSortedByIndex()
    {
        var chapter = new Chapter(2, "Al-Baqara", "البقرة");
        chapter.AddVerse(new Verse(3, "c"));
        chapter.AddVerse(new Verse(1, "a"));
        chapter.AddVerse(new Verse(2, "b"));

        Assert.Equal(new[] { 1, 2, 3 }, chapter.Verses.Select(verse => verse.Index));
    }
}
