using QuranX.DocumentModel;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class HadithDocumentVisitorTests
{
    private sealed class TrackingVisitor : HadithDocumentVisitor
    {
        public List<string> VisitedCollections { get; } = new();
        public List<string> VisitedHadiths { get; } = new();

        public void Run(HadithDocument document) => VisitDocument(document);

        protected override void VisitCollection(HadithCollection collection)
        {
            VisitedCollections.Add(collection.Code);
            base.VisitCollection(collection);
        }

        protected override void VisitHadith(Hadith hadith)
        {
            VisitedHadiths.Add(hadith.PrimaryReference[0]);
            base.VisitHadith(hadith);
        }
    }

    private static HadithCollection BuildCollection(string code)
    {
        return new HadithCollection(
            code: code,
            name: code,
            copyright: "",
            referenceDefinitions: [
                new HadithReferenceDefinition(
                    isPrimary: true,
                    code: "USC",
                    name: "USC",
                    partNames: ["Number"]),
            ]);
    }

    [Fact]
    public void VisitDocument_VisitsAllCollections()
    {
        var document = new HadithDocument();
        document.AddCollection(BuildCollection("Bukhari"));
        document.AddCollection(BuildCollection("Muslim"));

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Equal(2, visitor.VisitedCollections.Count);
    }

    [Fact]
    public void VisitDocument_VisitsHadithsGroupedByPrimaryReferenceFirstValue()
    {
        var collection = BuildCollection("Bukhari");
        collection.AddHadith(new Hadith(
            collection: collection,
            references: [new HadithReference("USC", ["1"], null)],
            arabicText: ["arabic"],
            englishText: ["english"],
            verseReferences: System.Array.Empty<VerseRangeReference>()));
        collection.AddHadith(new Hadith(
            collection: collection,
            references: [new HadithReference("USC", ["2"], null)],
            arabicText: ["arabic"],
            englishText: ["english"],
            verseReferences: System.Array.Empty<VerseRangeReference>()));

        var document = new HadithDocument();
        document.AddCollection(collection);

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Equal(2, visitor.VisitedHadiths.Count);
    }
}
