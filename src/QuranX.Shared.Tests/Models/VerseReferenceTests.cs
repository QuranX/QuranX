using QuranX.Shared.Models;

namespace QuranX.Shared.Tests.Models;

[Trait("Category", "Unit")]
public sealed class VerseReferenceTests
{
    [Fact]
    public void Parse_ChapterAndVerse_ReturnsExpected()
    {
        var reference = VerseReference.Parse("2.3");
        Assert.Equal(2, reference.Chapter);
        Assert.Equal(3, reference.Verse);
    }

    [Fact]
    public void GetDisplayText_ReturnsChapterDotVerse()
    {
        Assert.Equal("1.5", new VerseReference(1, 5).GetDisplayText());
    }

    [Fact]
    public void Equals_SameChapterAndVerse_IsTrue()
    {
        Assert.True(new VerseReference(1, 5) == new VerseReference(1, 5));
        Assert.True(new VerseReference(1, 5) != new VerseReference(1, 6));
    }

    [Fact]
    public void EqualityOperators_WithNullOperands_DoNotThrow()
    {
        VerseReference? nonNull = new VerseReference(1, 5);
        VerseReference? @null = null;

        Assert.False(@null == nonNull);
        Assert.False(nonNull == @null);
        Assert.True((VerseReference?)null == (VerseReference?)null);

        Assert.True(@null != nonNull);
        Assert.True(nonNull != @null);
        Assert.False((VerseReference?)null != (VerseReference?)null);
    }

    [Fact]
    public void CompareTo_OrdersByChapterThenVerse()
    {
        Assert.True(new VerseReference(1, 5).CompareTo(new VerseReference(2, 1)) < 0);
        Assert.True(new VerseReference(2, 1).CompareTo(new VerseReference(1, 5)) > 0);
        Assert.Equal(0, new VerseReference(2, 3).CompareTo(new VerseReference(2, 3)));
        Assert.True(new VerseReference(2, 3).CompareTo(null) > 0);
    }

    [Fact]
    public void GetIndexValue_EncodesChapterTimes1000PlusVerse()
    {
        Assert.Equal(2003, VerseReference.GetIndexValue(2, 3));
        Assert.Equal(5010, new VerseReference(5, 10).ToIndexValue());
    }

    [Fact]
    public void ToString_DelegatesToGetDisplayText()
    {
        Assert.Equal("7.42", new VerseReference(7, 42).ToString());
    }

    [Fact]
    public void IComparable_CompareTo_ThrowsOnWrongType()
    {
        IComparable comparable = new VerseReference(1, 1);
        Assert.Throws<ArgumentException>(() => comparable.CompareTo("not a verse"));
    }
}
