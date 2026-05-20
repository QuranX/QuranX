using QuranX.Persistence.Models;

namespace QuranX.Persistence.Tests.Models;

[Trait("Category", "Unit")]
public sealed class VerseTests
{
    private static Verse Make(
        int chapter,
        int verse,
        int hadiths = 0,
        int commentaries = 0,
        int roots = 0)
        => new(
            chapterNumber: chapter,
            verseNumber: verse,
            rootWordCount: roots,
            hadithCount: hadiths,
            commentaryCount: commentaries,
            verseTexts: []);

    [Fact]
    public void Constructor_ComputesIdFromChapterAndVerse()
    {
        Assert.Equal(2003, Make(2, 3).Id);
    }

    [Fact]
    public void Constructor_StoresCounts()
    {
        var verse = Make(1, 1, hadiths: 5, commentaries: 10, roots: 4);
        Assert.Equal(5, verse.HadithCount);
        Assert.Equal(10, verse.CommentaryCount);
        Assert.Equal(4, verse.RootWordCount);
    }

    [Fact]
    public void Equals_ComparesChapterAndVerseOnly()
    {
        Assert.True(Make(2, 3, hadiths: 1).Equals(Make(2, 3, hadiths: 99)));
        Assert.False(Make(2, 3).Equals(Make(2, 4)));
        Assert.False(Make(2, 3).Equals(Make(3, 3)));
    }

    [Fact]
    public void CompareTo_OrdersByChapterThenVerse()
    {
        Assert.Equal(-1, Make(1, 5).CompareTo(Make(2, 1)));
        Assert.Equal(1, Make(2, 1).CompareTo(Make(1, 5)));
        Assert.Equal(0, Make(2, 3).CompareTo(Make(2, 3)));
        Assert.Equal(-1, Make(2, 1).CompareTo(Make(2, 5)));
    }

    [Fact]
    public void VerseTexts_AreReadOnly()
    {
        var texts = new[]
        {
            new VerseText("EN", "Yusuf", "In the name of Allah..."),
            new VerseText("AR", "Arabic", "بسم الله"),
        };
        var verse = new Verse(1, 1, 0, 0, 0, texts);
        Assert.Equal(2, verse.VerseTexts.Count);
        Assert.IsAssignableFrom<IReadOnlyList<VerseText>>(verse.VerseTexts);
    }
}
