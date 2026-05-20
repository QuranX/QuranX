namespace QuranX.Persistence.Tests;

[Trait("Category", "Unit")]
public sealed class ArabicWordIndexerTests
{
    [Fact]
    public void GetIndexForArabic_SubstitutesAndReplacesHyphensWithX()
    {
        // بت -> "ba-ta" via ArabicHelper, then "-" -> "x".
        Assert.Equal("baxta", ArabicWordIndexer.GetIndexForArabic("بت"));
    }

    [Fact]
    public void GetIndexForArabic_AppliesSubstituteAndOmit()
    {
        // ء is omitted (substitutes to \0 -> dropped). "بءت" -> "بت" -> "ba-ta" -> "baxta".
        Assert.Equal("baxta", ArabicWordIndexer.GetIndexForArabic("بءت"));
    }

    [Fact]
    public void GetIndexForArabic_Collection_MapsEachWord()
    {
        var result = ArabicWordIndexer.GetIndexForArabic(new[] { "ب", "ت" }).ToList();
        Assert.Equal(new[] { "ba", "ta" }, result);
    }
}
