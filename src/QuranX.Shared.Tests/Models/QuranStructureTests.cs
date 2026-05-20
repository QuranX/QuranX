using QuranX.Shared.Models;

namespace QuranX.Shared.Tests.Models;

[Trait("Category", "Unit")]
public sealed class QuranStructureTests
{
    [Fact]
    public void Chapters_ContainsAll114()
    {
        Assert.Equal(114, QuranStructure.Chapters.Length);
    }

    [Fact]
    public void Chapter_1_IsAlFatihaWith7Verses()
    {
        var chapter = QuranStructure.Chapter(1);
        Assert.Equal(1, chapter.ChapterNumber);
        Assert.Equal(7, chapter.NumberOfVerses);
        Assert.Contains("Faatiha", chapter.EnglishName);
    }

    [Fact]
    public void Chapter_2_AlBaqara_Has286Verses()
    {
        Assert.Equal(286, QuranStructure.VerseCount(2));
    }

    [Theory]
    [InlineData(1, 1, true)]
    [InlineData(1, 7, true)]
    [InlineData(114, 6, true)]
    [InlineData(0, 1, false)]
    [InlineData(115, 1, false)]
    [InlineData(1, 0, false)]
    [InlineData(1, 8, false)]
    public void TryValidateChapterAndVerse_BoundsAreEnforced(
        int chapter, int verse, bool expected)
    {
        Assert.Equal(expected, QuranStructure.TryValidateChapterAndVerse(chapter, verse));
    }

    [Fact]
    public void Chapter_UnknownChapter_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => QuranStructure.Chapter(999));
    }
}
