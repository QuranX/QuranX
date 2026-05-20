using QuranX.DocumentModel;
using QuranX.DocumentModel.Factories;

namespace QuranX.DocumentModel.Tests.Factories;

[Trait("Category", "Unit")]
public sealed class HadithCollectionFactoryTests : IDisposable
{
    private readonly string _tempDir;
    private readonly string _hadithFile;
    private readonly string _xrefsDir;

    public HadithCollectionFactoryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        _hadithFile = Path.Combine(_tempDir, "Bukhari.xml");
        _xrefsDir = Path.Combine(_tempDir, "xrefs");
        Directory.CreateDirectory(_xrefsDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    private const string SampleXml = """
<hadithCollection>
    <code>Bukhari</code>
    <name>Sahih al-Bukhari</name>
    <copyright>PD</copyright>
    <referenceDefinitions>
        <referenceDefinition>
            <isPrimary>true</isPrimary>
            <code>USC</code>
            <name>USC-MSA</name>
            <valuePrefix></valuePrefix>
            <parts>
                <part>Volume</part>
                <part>Book</part>
                <part>Number</part>
            </parts>
        </referenceDefinition>
    </referenceDefinitions>
    <hadiths>
        <hadith>
            <references>
                <reference>
                    <code>USC</code>
                    <suffix></suffix>
                    <parts>
                        <part>1</part>
                        <part>1</part>
                        <part>1</part>
                    </parts>
                </reference>
            </references>
            <arabic>
                <text>arabic body</text>
            </arabic>
            <english>
                <text>english body</text>
            </english>
            <verseReferences>
                <reference>
                    <chapter>2</chapter>
                    <firstVerse>5</firstVerse>
                    <lastVerse>5</lastVerse>
                </reference>
            </verseReferences>
        </hadith>
    </hadiths>
</hadithCollection>
""";

    [Fact]
    public void Create_FromMinimalXml_ReturnsCollectionWithMetadata()
    {
        File.WriteAllText(_hadithFile, SampleXml);

        var factory = new HadithCollectionFactory();
        HadithCollection collection = factory.Create(_hadithFile, _xrefsDir);

        Assert.Equal("Bukhari", collection.Code);
        Assert.Equal("Sahih al-Bukhari", collection.Name);
        Assert.Equal("PD", collection.Copyright);
    }

    [Fact]
    public void Create_FromMinimalXml_LoadsReferenceDefinitions()
    {
        File.WriteAllText(_hadithFile, SampleXml);

        var factory = new HadithCollectionFactory();
        HadithCollection collection = factory.Create(_hadithFile, _xrefsDir);

        Assert.Single(collection.ReferenceDefinitions);
        Assert.True(collection.PrimaryReferenceDefinition.IsPrimary);
        Assert.Equal("USC", collection.PrimaryReferenceDefinition.Code);
    }

    [Fact]
    public void Create_FromMinimalXml_LoadsHadiths()
    {
        File.WriteAllText(_hadithFile, SampleXml);

        var factory = new HadithCollectionFactory();
        HadithCollection collection = factory.Create(_hadithFile, _xrefsDir);

        Assert.Single(collection.Hadiths);
        Hadith hadith = collection.Hadiths.Single();
        Assert.Equal("arabic body", hadith.ArabicText[0]);
        Assert.Equal("english body", hadith.EnglishText[0]);
    }

    [Fact]
    public void Create_WithXrefsFile_AddsAdditionalVerseReferences()
    {
        File.WriteAllText(_hadithFile, SampleXml);
        File.WriteAllText(
            Path.Combine(_xrefsDir, "Bukhari.txt"),
            "1.1.1\t3.10");

        var factory = new HadithCollectionFactory();
        HadithCollection collection = factory.Create(_hadithFile, _xrefsDir);

        Hadith hadith = collection.Hadiths.Single();
        Assert.Contains(hadith.VerseReferences, x => x.Chapter == 3);
    }

    [Fact]
    public void Create_WithXrefsRemoveRule_ExcludesVerseReferences()
    {
        File.WriteAllText(_hadithFile, SampleXml);
        File.WriteAllText(
            Path.Combine(_xrefsDir, "Bukhari.txt"),
            "1.1.1\tremove:2.5");

        var factory = new HadithCollectionFactory();
        HadithCollection collection = factory.Create(_hadithFile, _xrefsDir);

        Hadith hadith = collection.Hadiths.Single();
        Assert.DoesNotContain(hadith.VerseReferences, x => x.Chapter == 2 && x.FirstVerse == 5);
    }
}
