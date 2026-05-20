namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class ChapterAndVerseTests
{
    private static ChapterAndVerse Make(int chapter, int verse) =>
        new(new Chapter(chapter, "name", "name"), new Verse(verse, "ar"));

    [Fact]
    public void Equals_SameChapterAndVerse_IsTrue()
    {
        Assert.True(Make(1, 1).Equals(Make(1, 1)));
    }

    [Fact]
    public void Equals_DifferentVerse_IsFalse()
    {
        Assert.False(Make(1, 1).Equals(Make(1, 2)));
        Assert.False(Make(1, 1).Equals(Make(2, 1)));
    }

    [Fact]
    public void CompareTo_OrdersByChapterThenVerse()
    {
        Assert.Equal(-1, Make(1, 2).CompareTo(Make(2, 1)));
        Assert.Equal(1, Make(2, 1).CompareTo(Make(1, 2)));
        Assert.Equal(0, Make(1, 1).CompareTo(Make(1, 1)));
        Assert.Equal(-1, Make(1, 1).CompareTo(Make(1, 2)));
        Assert.Equal(1, Make(1, 2).CompareTo(Make(1, 1)));
    }

    [Fact]
    public void IComparable_ThrowsOnWrongType()
    {
        IComparable comparable = Make(1, 1);
        Assert.Throws<ArgumentException>(() => comparable.CompareTo("nope"));
    }
}
