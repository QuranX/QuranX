using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class CommentatorRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;

    public CommentatorRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
    }

    private void SeedCommentators()
    {
        _fixture.Reseed(writer =>
        {
            var writeRepo = new CommentatorWriteRepository(new StubWriterProvider(writer));
            writeRepo.Write(new Commentator("Kathir", "Ibn Kathir"));
            writeRepo.Write(new Commentator("Jalal", "Tafsir al-Jalalayn"));
        });
    }

    [Fact]
    public void GetAll_ReturnsAllCommentatorsOrderedByCode()
    {
        SeedCommentators();
        var repository = new CommentatorRepository(_fixture.SearcherProvider);

        var results = repository.GetAll().ToList();

        Assert.Equal(2, results.Count);
        Assert.Equal("Jalal", results[0].Code);
        Assert.Equal("Kathir", results[1].Code);
    }

    [Fact]
    public void TryGet_ExistingCode_ReturnsTrue()
    {
        SeedCommentators();
        var repository = new CommentatorRepository(_fixture.SearcherProvider);
        string code = "Kathir";

        bool found = repository.TryGet(ref code, out Commentator commentator);

        Assert.True(found);
        Assert.NotNull(commentator);
        Assert.Equal("Kathir", commentator.Code);
    }

    [Fact]
    public void TryGet_CaseInsensitive_ReturnsTrue()
    {
        SeedCommentators();
        var repository = new CommentatorRepository(_fixture.SearcherProvider);
        string code = "kathir";

        bool found = repository.TryGet(ref code, out Commentator commentator);

        Assert.True(found);
        Assert.Equal("Kathir", code);
    }

    [Fact]
    public void TryGet_UnknownCode_ReturnsFalse()
    {
        SeedCommentators();
        var repository = new CommentatorRepository(_fixture.SearcherProvider);
        string code = "Unknown";

        bool found = repository.TryGet(ref code, out Commentator commentator);

        Assert.False(found);
        Assert.Null(commentator);
    }
}
