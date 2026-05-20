using QuranX.DocumentModel;
using QuranX.DocumentModel.Factories;

namespace QuranX.DocumentModel.Tests.Factories;

[Trait("Category", "Unit")]
public sealed class QuranDocumentFactoryTests : IDisposable
{
    private readonly string _tempDir;

    public QuranDocumentFactoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Fact]
    public void Create_EmptyTranslationsDir_BuildsFullQuran()
    {
        var factory = new QuranDocumentFactory();
        QuranDocument document = factory.Create(_tempDir);

        Assert.Equal(114, document.ChapterCount);
    }

    [Fact]
    public void Create_AddsAllVersesInFirstChapter()
    {
        var factory = new QuranDocumentFactory();
        QuranDocument document = factory.Create(_tempDir);

        Chapter firstChapter = document[1];
        Assert.Equal(7, firstChapter.VerseCount);
    }

    [Fact]
    public void Create_WithSingleTranslationFile_AttachesTranslation()
    {
        const string translationXml = """
<quran>
    <translatorCode>TEST</translatorCode>
    <translatorName>Test Translator</translatorName>
    <chapter index="1">
        <verse index="1">In the name of</verse>
    </chapter>
</quran>
""";
        File.WriteAllText(Path.Combine(_tempDir, "test.xml"), translationXml);

        var factory = new QuranDocumentFactory();
        QuranDocument document = factory.Create(_tempDir);

        Verse verse = document[1, 1];
        Assert.Contains(verse.Translations, x => x.TranslatorCode == "TEST");
    }
}
