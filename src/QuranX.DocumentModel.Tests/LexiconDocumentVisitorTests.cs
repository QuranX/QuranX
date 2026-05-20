using QuranX.DocumentModel;

namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class LexiconDocumentVisitorTests
{
    private sealed class TrackingVisitor : LexiconDocumentVisitor
    {
        public List<string> VisitedLexicons { get; } = new();
        public List<char> VisitedLetters { get; } = new();
        public List<string> VisitedEntries { get; } = new();

        public void Run(LexiconDocument document) => VisitDocument(document);

        protected override void VisitLexicon(Lexicon lexicon)
        {
            VisitedLexicons.Add(lexicon.Code);
            base.VisitLexicon(lexicon);
        }

        protected override void VisitLetter(LexiconLetter letter)
        {
            VisitedLetters.Add(letter.Letter);
            base.VisitLetter(letter);
        }

        protected override void VisitEntry(LexiconEntry entry)
        {
            VisitedEntries.Add(entry.Root);
            base.VisitEntry(entry);
        }
    }

    [Fact]
    public void VisitDocument_VisitsAllLexiconsLettersAndEntries()
    {
        var document = new LexiconDocument();
        var lane = new Lexicon("Lane", "Lane");
        lane.AddEntry(new LexiconEntry("كتب", "to write"));
        lane.AddEntry(new LexiconEntry("علم", "to know"));
        document.AddLexicon(lane);

        var visitor = new TrackingVisitor();
        visitor.Run(document);

        Assert.Single(visitor.VisitedLexicons);
        Assert.Equal(2, visitor.VisitedLetters.Count);
        Assert.Equal(2, visitor.VisitedEntries.Count);
    }
}
