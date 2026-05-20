using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class VerseAnalysisRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;
    private readonly VerseAnalysisRepository _repository;

    public VerseAnalysisRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
        _repository = new VerseAnalysisRepository(_fixture.SearcherProvider);
    }

    private static VerseAnalysis MakeAnalysis(int chapter, int verse, params string[] roots)
    {
        var wordParts = roots
            .Select(root => new VerseAnalysisWordPart(
                root: root,
                type: "N",
                form: "form",
                description: "desc",
                decorators: []))
            .ToList();
        var word = new VerseAnalysisWord(
            wordNumber: 1,
            english: "word",
            buckwalter: "buck",
            wordParts: wordParts);
        return new VerseAnalysis(chapter, verse, [word]);
    }

    private void SeedAnalyses()
    {
        _fixture.Reseed(writer =>
        {
            var writeRepo = new VerseAnalysisWriteRepository(new StubWriterProvider(writer));
            writeRepo.Write(MakeAnalysis(1, 1, "كتب"));
            writeRepo.Write(MakeAnalysis(1, 2, "علم"));
            writeRepo.Write(MakeAnalysis(2, 5, "كتب"));
        });
    }

    [Fact]
    public void GetForVerse_ExistingVerse_ReturnsAnalysis()
    {
        SeedAnalyses();

        VerseAnalysis analysis = _repository.GetForVerse(chapterNumber: 1, verseNumber: 2);

        Assert.NotNull(analysis);
        Assert.Equal(1, analysis.ChapterNumber);
        Assert.Equal(2, analysis.VerseNumber);
    }

    [Fact]
    public void GetForRoot_KnownRoot_ReturnsAllOccurrences()
    {
        SeedAnalyses();

        var results = _repository.GetForRoot("كتب").ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void GetForRoot_UnknownRoot_ReturnsEmpty()
    {
        SeedAnalyses();

        var results = _repository.GetForRoot("xyz").ToList();
        Assert.Empty(results);
    }
}
