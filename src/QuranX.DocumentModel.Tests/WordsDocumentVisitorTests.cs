using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class WordsDocumentVisitorTests
{
    private sealed class TrackingVisitor : WordsDocumentVisitor
    {
        public List<string> VisitedRootWords { get; } = new();
        public List<string> VisitedReferences { get; } = new();

        public void Run(WordsDocument document) => VisitDocument(document);

        protected override void VisitRootWord(Word root)
        {
            VisitedRootWords.Add(root.Text);
            base.VisitRootWord(root);
        }

        protected override void VisitRootWordReference(WordReference reference)
        {
            VisitedReferences.Add(reference.LocationKey);
            base.VisitRootWordReference(reference);
        }
    }

    private static WordReference MakeReference(int chapter, int verse, int word) =>
        new(
            root: "كتب",
            chapterIndex: chapter,
            verseIndex: verse,
            wordIndex: word,
            wordPartIndex: 0,
            wordPartType: "N",
            wordPartTypeDescription: "noun",
            buckwalterText: "buck",
            englishText: "english");

    [Fact]
    public void VisitDocument_VisitsRootsAndReferences()
    {
        var document = new WordsDocument();
        document.AddRootWordReferenceGroup("كتب",
        [
            MakeReference(1, 1, 1),
            MakeReference(2, 5, 2),
        ]);
        document.AddRootWordReferenceGroup("علم",
        [
            MakeReference(3, 1, 1),
        ]);

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Equal(2, visitor.VisitedRootWords.Count);
        Assert.Equal(3, visitor.VisitedReferences.Count);
    }
}
