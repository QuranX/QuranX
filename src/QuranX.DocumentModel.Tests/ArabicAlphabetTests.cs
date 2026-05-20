namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class ArabicAlphabetTests
{
    [Fact]
    public void GetSimplifiedArabicChar_HamzaForms_NormaliseToAlif()
    {
        Assert.Equal("ا", ArabicAlphabet.GetSimplifiedArabicChar('إ'));
        Assert.Equal("ا", ArabicAlphabet.GetSimplifiedArabicChar('أ'));
        Assert.Equal("ا", ArabicAlphabet.GetSimplifiedArabicChar('آ'));
    }

    [Fact]
    public void GetSimplifiedArabicChar_TaaMarbouta_NormalisesToTaa()
    {
        Assert.Equal("ت", ArabicAlphabet.GetSimplifiedArabicChar('ة'));
    }

    [Fact]
    public void GetSimplifiedArabicChar_Unknown_Throws()
    {
        Assert.Throws<ArgumentException>(() => ArabicAlphabet.GetSimplifiedArabicChar('Z'));
    }

    [Fact]
    public void GetSimplifiedArabicString_Empty_ReturnsEmpty()
    {
        Assert.Equal(
            "",
            ArabicAlphabet.GetSimplifiedArabicString("", throwErrorOnUnknownCharacter: true));
    }

    [Fact]
    public void GetSimplifiedArabicString_UnknownChar_DroppedWhenNotThrowing()
    {
        Assert.Equal(
            "بت",
            ArabicAlphabet.GetSimplifiedArabicString("بZت", throwErrorOnUnknownCharacter: false));
    }

    [Fact]
    public void GetSimplifiedArabicString_UnknownChar_ThrowsWhenRequested()
    {
        Assert.Throws<KeyNotFoundException>(() =>
            ArabicAlphabet.GetSimplifiedArabicString("بZت", throwErrorOnUnknownCharacter: true));
    }
}
