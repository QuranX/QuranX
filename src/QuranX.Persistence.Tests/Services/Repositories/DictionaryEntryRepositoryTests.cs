using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class DictionaryEntryRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;
    private readonly DictionaryEntryRepository _repository;

    public DictionaryEntryRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
        _repository = new DictionaryEntryRepository(_fixture.SearcherProvider);
    }

    private void SeedEntries()
    {
        _fixture.Reseed(writer =>
        {
            var writeRepo = new DictionaryEntryWriteRepository(new StubWriterProvider(writer));
            writeRepo.Write(new DictionaryEntry("Lane", "كتب", entryIndex: 1, html: ["lane entry"]));
            writeRepo.Write(new DictionaryEntry("Lane", "كتاب", entryIndex: 2, html: ["lane entry 2"]));
            writeRepo.Write(new DictionaryEntry("Hans", "كتب", entryIndex: 1, html: ["hans entry"]));
        });
    }

    [Fact]
    public void Get_ByWord_ReturnsMatchingEntriesAcrossDictionaries()
    {
        SeedEntries();

        var results = _repository.Get("كتب").ToList();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void Get_ByDictionaryAndWord_FiltersByDictionary()
    {
        SeedEntries();

        var results = _repository.Get("Lane", "كتب").ToList();

        Assert.Single(results);
        Assert.Equal("Lane", results[0].DictionaryCode);
    }

    [Fact]
    public void GetAll_ReturnsDistinctWordsOrdered()
    {
        SeedEntries();

        var words = _repository.GetAll().ToList();

        Assert.Equal(2, words.Count);
    }

    [Fact]
    public void GetNextRoots_EmptyRoot_ReturnsAlphabetLetters()
    {
        SeedEntries();

        var roots = _repository.GetNextRoots(string.Empty).ToList();

        Assert.NotEmpty(roots);
        Assert.Equal(28, roots.Count);
    }

    [Fact]
    public void GetNextRoots_WithRoot_ReturnsPrefixedExtensions()
    {
        SeedEntries();

        var roots = _repository.GetNextRoots("ك").ToList();

        Assert.All(roots, root => Assert.StartsWith("ك", root));
    }
}
