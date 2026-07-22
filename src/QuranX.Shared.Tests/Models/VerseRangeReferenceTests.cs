using QuranX.Shared.Models;

namespace QuranX.Shared.Tests.Models;

[Trait("Category", "Unit")]
public sealed class VerseRangeReferenceTests
{
    [Fact]
    public void Parse_SingleVerse_FirstEqualsLast()
    {
        var range = VerseRangeReference.Parse("2.3");
        Assert.Equal(2, range.Chapter);
        Assert.Equal(3, range.FirstVerse);
        Assert.Equal(3, range.LastVerse);
    }

    [Fact]
    public void Parse_VerseRange_ReturnsFirstAndLast()
    {
        var range = VerseRangeReference.Parse("2.3-5");
        Assert.Equal(2, range.Chapter);
        Assert.Equal(3, range.FirstVerse);
        Assert.Equal(5, range.LastVerse);
    }

    [Fact]
    public void Parse_Malformed_ThrowsFormatExceptionWithContext()
    {
        var exception = Assert.Throws<FormatException>(() => VerseRangeReference.Parse("not-a-reference"));
        Assert.Contains("not-a-reference", exception.Message);
    }

    [Theory]
    [InlineData("2.3")]
    [InlineData("2.3-5")]
    public void TryParse_ValidReference_ReturnsTrue(string source)
    {
        Assert.True(VerseRangeReference.TryParse(source, out var range));
        Assert.NotNull(range);
    }

    [Fact]
    public void TryParse_SingleVerse_PopulatesFirstEqualsLast()
    {
        Assert.True(VerseRangeReference.TryParse("2.3", out var range));
        Assert.Equal(2, range!.Chapter);
        Assert.Equal(3, range.FirstVerse);
        Assert.Equal(3, range.LastVerse);
    }

    [Fact]
    public void TryParse_VerseRange_PopulatesFirstAndLast()
    {
        Assert.True(VerseRangeReference.TryParse("2.3-5", out var range));
        Assert.Equal(2, range!.Chapter);
        Assert.Equal(3, range.FirstVerse);
        Assert.Equal(5, range.LastVerse);
    }

    [Fact]
    public void TryParse_ZeroLastVerse_NormalisesToFirstVerse()
    {
        Assert.True(VerseRangeReference.TryParse("2.3-0", out var range));
        Assert.Equal(3, range!.FirstVerse);
        Assert.Equal(3, range.LastVerse);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("garbage")]
    [InlineData("2")]
    [InlineData("2.")]
    [InlineData(".3")]
    [InlineData("2.a")]
    [InlineData("a.3")]
    [InlineData("2.3.4")]
    [InlineData("2.3-4-5")]
    public void TryParse_MalformedInput_ReturnsFalse(string? source)
    {
        Assert.False(VerseRangeReference.TryParse(source, out var range));
        Assert.Null(range);
    }

    [Theory]
    [InlineData("0.1")]     // chapter below range
    [InlineData("115.1")]   // chapter above range (only 114 chapters)
    [InlineData("999.1")]   // chapter far above range
    [InlineData("2.0")]     // verse below range
    [InlineData("1.8")]     // verse above range (chapter 1 has 7 verses)
    [InlineData("1.1-8")]   // last verse above range
    public void TryParse_OutOfRange_ReturnsFalse(string source)
    {
        Assert.False(VerseRangeReference.TryParse(source, out var range));
        Assert.Null(range);
    }

    [Fact]
    public void TryParse_InvertedRange_ReturnsFalse()
    {
        Assert.False(VerseRangeReference.TryParse("2.5-3", out var range));
        Assert.Null(range);
    }

    [Fact]
    public void LastVerse_ZeroNormalisesToFirstVerse()
    {
        Assert.Equal(3, new VerseRangeReference(2, 3, 0).LastVerse);
    }

    [Fact]
    public void IsMultipleVerses_TrueWhenFirstNotEqualLast()
    {
        Assert.True(new VerseRangeReference(2, 3, 5).IsMultipleVerses());
        Assert.False(new VerseRangeReference(2, 3, 3).IsMultipleVerses());
    }

    [Fact]
    public void GetDisplayText_SingleVerse_OmitsRangeSuffix()
    {
        Assert.Equal("2.3", new VerseRangeReference(2, 3, 3).GetDisplayText());
        Assert.Equal("2.3-5", new VerseRangeReference(2, 3, 5).GetDisplayText());
    }

    [Fact]
    public void Includes_ReturnsTrueForVerseInsideRange()
    {
        var range = new VerseRangeReference(2, 3, 5);
        Assert.True(range.Includes(2, 3));
        Assert.True(range.Includes(2, 4));
        Assert.True(range.Includes(2, 5));
        Assert.False(range.Includes(2, 6));
        Assert.False(range.Includes(2, 2));
        Assert.False(range.Includes(3, 4));
    }

    [Fact]
    public void ToVerseReferences_ExpandsRangeIntoIndividualVerses()
    {
        var references = new VerseRangeReference(2, 3, 5).ToVerseReferences().ToList();
        Assert.Equal(3, references.Count);
        Assert.Equal(new VerseReference(2, 3), references[0]);
        Assert.Equal(new VerseReference(2, 4), references[1]);
        Assert.Equal(new VerseReference(2, 5), references[2]);
    }

    [Fact]
    public void GetIndexValue_PacksChapterFirstLast()
    {
        Assert.Equal(2_003_005, VerseRangeReference.GetIndexValue(2, 3, 5));
    }

    [Fact]
    public void CompareTo_OrdersByChapterFirstThenFirstVerseThenLastVerse()
    {
        Assert.True(
            new VerseRangeReference(1, 1, 3).CompareTo(new VerseRangeReference(1, 1, 5)) < 0);
        Assert.True(
            new VerseRangeReference(2, 1, 1).CompareTo(new VerseRangeReference(1, 100, 100)) > 0);
    }

    [Fact]
    public void Simplify_MergesAdjacentVersesIntoRanges()
    {
        var input = new[]
        {
            new VerseRangeReference(1, 1, 1),
            new VerseRangeReference(1, 2, 2),
            new VerseRangeReference(1, 3, 3),
            new VerseRangeReference(1, 7, 7),
        };
        var simplified = VerseRangeReference.Simplify(input).ToList();

        Assert.Equal(2, simplified.Count);
        Assert.Equal(1, simplified[0].Chapter);
        Assert.Equal(1, simplified[0].FirstVerse);
        Assert.Equal(3, simplified[0].LastVerse);
        Assert.Equal(1, simplified[1].Chapter);
        Assert.Equal(7, simplified[1].FirstVerse);
        Assert.Equal(7, simplified[1].LastVerse);
    }

    [Fact]
    public void Simplify_ExclusionRemovesMatchingVerses()
    {
        var input = new[] { new VerseRangeReference(1, 1, 5) };
        var excluded = new[] { new VerseRangeReference(1, 3, 3) };
        var result = VerseRangeReference.Simplify(input, excluded).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].FirstVerse);
        Assert.Equal(2, result[0].LastVerse);
        Assert.Equal(4, result[1].FirstVerse);
        Assert.Equal(5, result[1].LastVerse);
    }
}
