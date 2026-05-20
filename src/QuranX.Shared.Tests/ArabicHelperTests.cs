namespace QuranX.Shared.Tests;

[Trait("Category", "Unit")]
public sealed class ArabicHelperTests
{
    [Fact]
    public void ArabicToLetterNames_KnownLetters_ReturnsHyphenated()
    {
        Assert.Equal("ba-ta", ArabicHelper.ArabicToLetterNames("بت"));
    }

    [Fact]
    public void ArabicToLetterNames_UnknownButPermittedChar_Throws()
    {
        // ؤ is in PermittedChars (so survives SubstituteAndOmit) but absent from
        // LetterNames -> throws.
        Assert.Throws<ArgumentException>(() => ArabicHelper.ArabicToLetterNames("ؤ"));
    }

    [Fact]
    public void LetterNamesToArabic_RoundTripsCommonLetters()
    {
        Assert.Equal("ا", ArabicHelper.LetterNamesToArabic("alif"));
        Assert.Equal("بت", ArabicHelper.LetterNamesToArabic("ba-ta"));
    }

    [Fact]
    public void LetterNamesToArabic_UnknownName_ReturnsNull()
    {
        Assert.Null(ArabicHelper.LetterNamesToArabic("nope"));
    }

    [Fact]
    public void Substitute_MapsKnownAlternates()
    {
        Assert.Equal("ا", ArabicHelper.Substitute("آ"));
        Assert.Equal("ت", ArabicHelper.Substitute("ة"));
    }

    [Fact]
    public void Substitute_EmptyString_ReturnsEmpty()
    {
        Assert.Equal("", ArabicHelper.Substitute(""));
    }

    [Fact]
    public void SubstituteAndOmit_DropsHamza()
    {
        // ء maps to '\0' in AlternateChars -> omitted.
        Assert.Equal("بت", ArabicHelper.SubstituteAndOmit("بءت"));
    }
}
