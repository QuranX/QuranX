using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class QuranDocumentVisitorTests
{
    private sealed class TrackingVisitor : QuranDocumentVisitor
    {
        public List<int> VisitedChapters { get; } = new();
        public List<int> VisitedVerses { get; } = new();
        public List<string> VisitedTranslators { get; } = new();

        public void Run(QuranDocument document) => VisitQuran(document);

        protected override void VisitChapter(Chapter chapter)
        {
            VisitedChapters.Add(chapter.Number);
            base.VisitChapter(chapter);
        }

        protected override void VisitVerse(Verse verse)
        {
            VisitedVerses.Add(verse.Index);
            base.VisitVerse(verse);
        }

        protected override void VisitVerseTranslations(IEnumerable<VerseTranslation> translations)
        {
            foreach (var translation in translations)
            {
                VisitedTranslators.Add(translation.TranslatorCode);
            }
            base.VisitVerseTranslations(translations);
        }
    }

    private static QuranDocument BuildDocument()
    {
        var document = new QuranDocument();
        var chapter1 = new Chapter(number: 1, englishName: "Opening", arabicName: "الفاتحة");
        var verse1 = new Verse(index: 1, arabicText: "بسم");
        verse1.AddTranslation(new VerseTranslation("EN", "English", "in the name"));
        chapter1.AddVerse(verse1);
        document.AddChapter(chapter1);

        var chapter2 = new Chapter(number: 2, englishName: "Cow", arabicName: "البقرة");
        chapter2.AddVerse(new Verse(index: 1, arabicText: "الم"));
        chapter2.AddVerse(new Verse(index: 2, arabicText: "ذلك"));
        document.AddChapter(chapter2);
        return document;
    }

    [Fact]
    public void VisitQuran_VisitsAllChapters()
    {
        var visitor = new TrackingVisitor();
        visitor.Run(BuildDocument());
        Assert.Equal([1, 2], visitor.VisitedChapters);
    }

    [Fact]
    public void VisitQuran_VisitsAllVerses()
    {
        var visitor = new TrackingVisitor();
        visitor.Run(BuildDocument());
        Assert.Equal(3, visitor.VisitedVerses.Count);
    }

    [Fact]
    public void VisitQuran_VisitsAllTranslations()
    {
        var visitor = new TrackingVisitor();
        visitor.Run(BuildDocument());
        Assert.Equal(["EN"], visitor.VisitedTranslators);
    }
}
