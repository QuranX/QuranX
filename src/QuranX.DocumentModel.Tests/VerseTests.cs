namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class VerseTests
{
    [Fact]
    public void Constructor_StoresIndexAndArabic()
    {
        var verse = new Verse(5, "نص عربي");
        Assert.Equal(5, verse.Index);
        Assert.Equal("نص عربي", verse.ArabicText);
        Assert.Empty(verse.Translations);
    }

    [Fact]
    public void AddTranslation_AppearsInTranslations()
    {
        var verse = new Verse(1, "x");
        verse.AddTranslation(new VerseTranslation("EN", "YusufAli", "In the name..."));
        Assert.Single(verse.Translations);
        Assert.Equal("EN", verse.Translations.Single().TranslatorCode);
    }

    [Fact]
    public void Translations_AreSortedByTranslatorName()
    {
        var verse = new Verse(1, "x");
        verse.AddTranslation(new VerseTranslation("Z", "Zahid", "z"));
        verse.AddTranslation(new VerseTranslation("A", "Ali", "a"));
        verse.AddTranslation(new VerseTranslation("M", "Maududi", "m"));

        Assert.Equal(
            new[] { "Ali", "Maududi", "Zahid" },
            verse.Translations.Select(translation => translation.TranslatorName));
    }
}
