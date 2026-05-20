using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class CommentaryRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;
    private readonly CommentaryRepository _repository;

    public CommentaryRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
        _repository = new CommentaryRepository(_fixture.SearcherProvider);
    }

    private void SeedCommentaries()
    {
        _fixture.Reseed(writer =>
        {
            var commentaryWriter = new CommentaryWriteRepository(new StubWriterProvider(writer));
            commentaryWriter.Write(new Commentary(
                commentatorCode: "Kathir",
                chapterNumber: 1,
                firstVerseNumber: 1,
                lastVerseNumber: 5,
                text: [new TextContent("first kathir entry", isArabic: false)]));
            commentaryWriter.Write(new Commentary(
                commentatorCode: "Jalal",
                chapterNumber: 1,
                firstVerseNumber: 3,
                lastVerseNumber: 4,
                text: [new TextContent("jalal entry", isArabic: false)]));
            commentaryWriter.Write(new Commentary(
                commentatorCode: "Kathir",
                chapterNumber: 2,
                firstVerseNumber: 1,
                lastVerseNumber: 1,
                text: [new TextContent("second kathir", isArabic: false)]));
        });
    }

    [Fact]
    public void GetForVerse_ChapterVerse_ReturnsAllMatchingCommentaries()
    {
        SeedCommentaries();

        var results = _repository.GetForVerse(chapterNumber: 1, verseNumber: 3).ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, x => x.CommentatorCode == "Kathir");
        Assert.Contains(results, x => x.CommentatorCode == "Jalal");
    }

    [Fact]
    public void GetForVerse_OutOfRange_ReturnsEmpty()
    {
        SeedCommentaries();

        var results = _repository.GetForVerse(chapterNumber: 1, verseNumber: 99).ToList();
        Assert.Empty(results);
    }

    [Fact]
    public void GetForVerse_OrderedByCommentatorThenChapter()
    {
        SeedCommentaries();

        var results = _repository.GetForVerse(chapterNumber: 1, verseNumber: 3).ToList();

        Assert.Equal("Jalal", results[0].CommentatorCode);
        Assert.Equal("Kathir", results[1].CommentatorCode);
    }

    [Fact]
    public void GetForVerse_WithCommentatorCode_ReturnsSingleMatch()
    {
        SeedCommentaries();

        Commentary result = _repository.GetForVerse(
            commentatorCode: "Kathir",
            chapterNumber: 1,
            verseNumber: 3);

        Assert.NotNull(result);
        Assert.Equal("Kathir", result.CommentatorCode);
    }

    [Fact]
    public void GetForVerse_WithCommentatorCode_NoMatch_ReturnsNull()
    {
        SeedCommentaries();

        Commentary result = _repository.GetForVerse(
            commentatorCode: "Kathir",
            chapterNumber: 9,
            verseNumber: 9);

        Assert.Null(result);
    }

    [Fact]
    public void GetForVerse_NullCommentatorCode_Throws()
    {
        SeedCommentaries();

        Assert.Throws<ArgumentNullException>(() =>
            _repository.GetForVerse(commentatorCode: null!, chapterNumber: 1, verseNumber: 1));
    }

    [Fact]
    public void GetVerseRangeReferences_ReturnsAllRangesForCommentator()
    {
        SeedCommentaries();

        var references = _repository.GetVerseRangeReferences("Kathir").ToList();

        Assert.Equal(2, references.Count);
    }

    [Fact]
    public void GetVerseRangeReferences_NullCode_Throws()
    {
        SeedCommentaries();

        Assert.Throws<ArgumentNullException>(() =>
            _repository.GetVerseRangeReferences(commentatorCode: null!));
    }

    [Fact]
    public void GetVerseRangeReferences_Cached_ReturnsSameInstance()
    {
        SeedCommentaries();

        var firstCall = _repository.GetVerseRangeReferences("Kathir");
        var secondCall = _repository.GetVerseRangeReferences("Kathir");

        Assert.Same(firstCall, secondCall);
    }
}
