using QuranX.DocumentModel;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class TafsirDocumentVisitorTests
{
    private sealed class TrackingVisitor : TafsirDocumentVisitor
    {
        public List<string> VisitedTafsirs { get; } = new();
        public List<int> VisitedChapters { get; } = new();
        public List<VerseRangeReference> VisitedComments { get; } = new();

        public void Run(TafsirDocument document) => VisitDocument(document);

        protected override void VisitTafsir(Tafsir tafsir)
        {
            VisitedTafsirs.Add(tafsir.Code);
            base.VisitTafsir(tafsir);
        }

        protected override void VisitChapter(int chapterIndex, IEnumerable<TafsirComment> comments)
        {
            VisitedChapters.Add(chapterIndex);
            base.VisitChapter(chapterIndex, comments);
        }

        protected override void VisitComment(TafsirComment comment)
        {
            VisitedComments.Add(comment.VerseReference);
            base.VisitComment(comment);
        }
    }

    private static Tafsir BuildTafsir(string code, params VerseRangeReference[] references)
    {
        var tafsir = new Tafsir(code, code, isTafsir: true, copyright: "");
        foreach (var reference in references)
        {
            tafsir.AddComment(new TafsirComment(reference, ["text"]));
        }
        return tafsir;
    }

    [Fact]
    public void VisitDocument_VisitsAllTafsirs()
    {
        var document = new TafsirDocument();
        document.AddTafsir(BuildTafsir("Kathir", new VerseRangeReference(1, 1, 1)));
        document.AddTafsir(BuildTafsir("Jalal", new VerseRangeReference(1, 1, 1)));

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Equal(2, visitor.VisitedTafsirs.Count);
    }

    [Fact]
    public void VisitDocument_VisitsChaptersInOrder()
    {
        var document = new TafsirDocument();
        document.AddTafsir(BuildTafsir("Kathir",
            new VerseRangeReference(3, 1, 1),
            new VerseRangeReference(1, 1, 1),
            new VerseRangeReference(2, 1, 1)));

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Equal([1, 2, 3], visitor.VisitedChapters);
    }

    [Fact]
    public void VisitDocument_VisitsAllComments()
    {
        var document = new TafsirDocument();
        document.AddTafsir(BuildTafsir("Kathir",
            new VerseRangeReference(1, 1, 1),
            new VerseRangeReference(1, 2, 2)));

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Equal(2, visitor.VisitedComments.Count);
    }
}
