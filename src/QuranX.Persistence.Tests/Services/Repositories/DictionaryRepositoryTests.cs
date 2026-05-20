using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;
using PersistenceDictionary = QuranX.Persistence.Models.Dictionary;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class DictionaryRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;

    public DictionaryRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
    }

    private void SeedDictionaries()
    {
        _fixture.Reseed(writer =>
        {
            var writeRepo = new DictionaryWriteRepository(new StubWriterProvider(writer));
            writeRepo.Write(new PersistenceDictionary("Lane", "Lane's Lexicon", "Public Domain"));
            writeRepo.Write(new PersistenceDictionary("Hans", "Hans Wehr", "Restricted"));
        });
    }

    [Fact]
    public void GetAll_ReturnsAllDictionariesOrderedByCode()
    {
        SeedDictionaries();
        var repository = new DictionaryRepository(_fixture.SearcherProvider);

        var results = repository.GetAll().ToList();

        Assert.Equal(2, results.Count);
        Assert.Equal("Hans", results[0].Code);
        Assert.Equal("Lane", results[1].Code);
    }

    [Fact]
    public void Get_ExistingCode_ReturnsDictionary()
    {
        SeedDictionaries();
        var repository = new DictionaryRepository(_fixture.SearcherProvider);
        string code = "Lane";

        PersistenceDictionary dictionary = repository.Get(ref code);

        Assert.NotNull(dictionary);
        Assert.Equal("Lane", dictionary.Code);
    }

    [Fact]
    public void Get_CaseInsensitive_NormalizesCode()
    {
        SeedDictionaries();
        var repository = new DictionaryRepository(_fixture.SearcherProvider);
        string code = "lane";

        PersistenceDictionary dictionary = repository.Get(ref code);

        Assert.NotNull(dictionary);
        Assert.Equal("Lane", code);
    }

    [Fact]
    public void Get_UnknownCode_ReturnsNull()
    {
        SeedDictionaries();
        var repository = new DictionaryRepository(_fixture.SearcherProvider);
        string code = "Unknown";

        PersistenceDictionary dictionary = repository.Get(ref code);

        Assert.Null(dictionary);
    }
}
