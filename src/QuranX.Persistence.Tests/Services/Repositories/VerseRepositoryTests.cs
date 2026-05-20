using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Tests.Services.Repositories;

[Trait("Category", "Unit")]
public sealed class VerseRepositoryTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;
    private readonly VerseRepository _repository;

    public VerseRepositoryTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
        _repository = new VerseRepository(_fixture.SearcherProvider);
    }

    private void SeedVerses()
    {
        _fixture.Reseed(writer =>
        {
            var writeRepo = new VerseWriteRepository(new StubWriterProvider(writer));
            writeRepo.Write(new Verse(
                chapterNumber: 1,
                verseNumber: 1,
                rootWordCount: 0,
                hadithCount: 0,
                commentaryCount: 0,
                verseTexts: [new VerseText("EN", "English", "first verse")]));
            writeRepo.Write(new Verse(
                chapterNumber: 1,
                verseNumber: 2,
                rootWordCount: 0,
                hadithCount: 0,
                commentaryCount: 0,
                verseTexts: [new VerseText("EN", "English", "second verse")]));
            writeRepo.Write(new Verse(
                chapterNumber: 2,
                verseNumber: 1,
                rootWordCount: 0,
                hadithCount: 0,
                commentaryCount: 0,
                verseTexts: [new VerseText("EN", "English", "third verse")]));
        });
    }

    [Fact]
    public void GetVerseReferences_ReturnsAllReferencesFromQuranStructure()
    {
        var references = _repository.GetVerseReferences().ToList();

        Assert.True(references.Count > 6000);
        Assert.Contains(references, reference => reference.Chapter == 1 && reference.Verse == 1);
    }

    [Fact]
    public void GetVerseReferences_CachesResult()
    {
        var firstCall = _repository.GetVerseReferences();
        var secondCall = _repository.GetVerseReferences();
        Assert.Same(firstCall, secondCall);
    }

    [Fact]
    public void GetVerse_KnownReference_ReturnsVerse()
    {
        SeedVerses();
        var repository = new VerseRepository(_fixture.SearcherProvider);

        Verse verse = repository.GetVerse(new VerseReference(1, 1));

        Assert.NotNull(verse);
        Assert.Equal(1, verse.ChapterNumber);
        Assert.Equal(1, verse.VerseNumber);
    }

    [Fact]
    public void GetVerses_Range_ReturnsAllVersesInRange()
    {
        SeedVerses();
        var repository = new VerseRepository(_fixture.SearcherProvider);

        var verses = repository.GetVerses([new VerseRangeReference(1, 1, 2)]).ToList();

        Assert.Equal(2, verses.Count);
    }

    [Fact]
    public void GetVerses_MultipleRanges_DeduplicatesResults()
    {
        SeedVerses();
        var repository = new VerseRepository(_fixture.SearcherProvider);

        var verses = repository.GetVerses(
        [
            new VerseRangeReference(1, 1, 1),
            new VerseRangeReference(1, 1, 1),
        ]).ToList();

        Assert.Single(verses);
    }
}
