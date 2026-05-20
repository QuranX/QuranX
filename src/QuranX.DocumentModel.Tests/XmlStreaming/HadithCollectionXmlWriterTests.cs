using QuranX.DocumentModel;
using QuranX.DocumentModel.XmlStreaming;
using QuranX.Shared.Models;
using System.Xml.Linq;

namespace QuranX.DocumentModel.Tests.XmlStreaming;

[Trait("Category", "Unit")]
public sealed class HadithCollectionXmlWriterTests : IDisposable
{
    private readonly string _tempFile = Path.GetTempFileName();

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }

    private static HadithCollection BuildCollection()
    {
        var collection = new HadithCollection(
            code: "Bukhari",
            name: "Sahih al-Bukhari",
            copyright: "PD",
            referenceDefinitions: [
                new HadithReferenceDefinition(
                    isPrimary: true,
                    code: "USC",
                    name: "USC-MSA",
                    partNames: ["Volume", "Book", "Number"],
                    valuePrefix: "p"),
                new HadithReferenceDefinition(
                    isPrimary: false,
                    code: "InBook",
                    name: "In Book",
                    partNames: ["Book", "Number"]),
            ]);
        collection.AddHadith(new Hadith(
            collection: collection,
            references: [
                new HadithReference("USC", ["1", "1", "1"], "a"),
                new HadithReference("InBook", ["1", "1"], null),
            ],
            arabicText: ["a1", "a2"],
            englishText: ["e1"],
            verseReferences: [new VerseRangeReference(2, 5, 7)]));
        return collection;
    }

    [Fact]
    public void WriteXml_ProducesParseableXml()
    {
        var writer = new HadithCollectionXmlWriter(BuildCollection());
        writer.WriteXml(_tempFile);

        XDocument document = XDocument.Load(_tempFile);
        Assert.Equal("hadithCollection", document.Root!.Name.LocalName);
        Assert.Equal("Bukhari", document.Root.Element("code")!.Value);
        Assert.Equal("Sahih al-Bukhari", document.Root.Element("name")!.Value);
        Assert.Equal("PD", document.Root.Element("copyright")!.Value);
    }

    [Fact]
    public void WriteXml_EmitsReferenceDefinitionsInPriorityOrder()
    {
        var writer = new HadithCollectionXmlWriter(BuildCollection());
        writer.WriteXml(_tempFile);

        XDocument document = XDocument.Load(_tempFile);
        var codes = document
            .Descendants("referenceDefinition")
            .Select(x => x.Element("code")!.Value)
            .ToList();

        Assert.Equal(["USC", "InBook"], codes);
    }

    [Fact]
    public void WriteXml_EmitsHadithReferencesOrderedByCode()
    {
        var writer = new HadithCollectionXmlWriter(BuildCollection());
        writer.WriteXml(_tempFile);

        XDocument document = XDocument.Load(_tempFile);
        var codes = document
            .Descendants("hadith")
            .Single()
            .Element("references")!
            .Elements("reference")
            .Select(x => x.Element("code")!.Value)
            .ToList();

        Assert.Equal(["InBook", "USC"], codes);
    }

    [Fact]
    public void WriteXml_EmitsArabicAndEnglishParagraphs()
    {
        var writer = new HadithCollectionXmlWriter(BuildCollection());
        writer.WriteXml(_tempFile);

        XDocument document = XDocument.Load(_tempFile);
        var hadith = document.Descendants("hadith").Single();
        Assert.Equal(2, hadith.Element("arabic")!.Elements("text").Count());
        Assert.Single(hadith.Element("english")!.Elements("text"));
    }

    [Fact]
    public void WriteXml_EmitsVerseReferences()
    {
        var writer = new HadithCollectionXmlWriter(BuildCollection());
        writer.WriteXml(_tempFile);

        XDocument document = XDocument.Load(_tempFile);
        var reference = document
            .Descendants("verseReferences")
            .Single()
            .Element("reference")!;

        Assert.Equal("2", reference.Element("chapter")!.Value);
        Assert.Equal("5", reference.Element("firstVerse")!.Value);
        Assert.Equal("7", reference.Element("lastVerse")!.Value);
    }
}
