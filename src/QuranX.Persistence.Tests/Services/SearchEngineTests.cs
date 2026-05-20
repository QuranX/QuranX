using Lucene.Net.Index;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services;
using QuranX.Persistence.Services.Repositories;
using QuranX.Persistence.Tests.Infrastructure;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Tests.Services;

[Trait("Category", "Unit")]
public sealed class SearchEngineTests : IClassFixture<LuceneIndexFixture>
{
    private readonly LuceneIndexFixture _fixture;
    private readonly SearchEngine _searchEngine;

    public SearchEngineTests(LuceneIndexFixture fixture)
    {
        _fixture = fixture;
        _searchEngine = new SearchEngine(_fixture.AnalyzerProvider, _fixture.SearcherProvider);
    }

    private void SeedMixedDocuments()
    {
        _fixture.Reseed(writer =>
        {
            var verseWriter = new VerseWriteRepository(new StubWriter(writer));
            verseWriter.Write(new Verse(
                chapterNumber: 1,
                verseNumber: 1,
                rootWordCount: 0,
                hadithCount: 0,
                commentaryCount: 0,
                verseTexts: [new VerseText("EN", "English", "guidance for the righteous")]));

            var commentaryWriter = new CommentaryWriteRepository(new StubWriter(writer));
            commentaryWriter.Write(new Commentary(
                commentatorCode: "Kathir",
                chapterNumber: 1,
                firstVerseNumber: 1,
                lastVerseNumber: 1,
                text: [new TextContent("explanation of guidance verses", isArabic: false)]));
            commentaryWriter.Write(new Commentary(
                commentatorCode: "Jalal",
                chapterNumber: 2,
                firstVerseNumber: 1,
                lastVerseNumber: 1,
                text: [new TextContent("alternative guidance interpretation", isArabic: false)]));

            var hadithWriter = new HadithWriteRepository(new StubWriter(writer));
            hadithWriter.Write(new Hadith(
                collectionCode: "Bukhari",
                arabicText: ["ال"],
                englishText: ["seek guidance from the wise"],
                verseRangeReferences: [],
                references: [],
                primaryReferenceCode: "USC",
                primaryReferencePath: "Bukhari/USC/1"));
            hadithWriter.Write(new Hadith(
                collectionCode: "Muslim",
                arabicText: ["ال"],
                englishText: ["guidance from the prophet"],
                verseRangeReferences: [],
                references: [],
                primaryReferenceCode: "USC",
                primaryReferencePath: "Muslim/USC/1"));
        });
    }

    [Fact]
    public void Search_EmptyQuery_ReturnsEmpty()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "",
            context: SearchContexts.WholeSite,
            subContext: "",
            out int total,
            maxResults: 10).ToList();

        Assert.Empty(results);
        Assert.Equal(0, total);
    }

    [Fact]
    public void Search_NullQuery_ReturnsEmpty()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: null!,
            context: SearchContexts.WholeSite,
            subContext: "",
            out int total,
            maxResults: 10).ToList();

        Assert.Empty(results);
        Assert.Equal(0, total);
    }

    [Fact]
    public void Search_QuranContext_ReturnsOnlyVerses()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.Quran,
            subContext: "",
            out int total,
            maxResults: 10).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, result => Assert.Equal(nameof(Verse), result.Type));
        Assert.Equal(results.Count, total);
    }

    [Fact]
    public void Search_CommentariesContext_ReturnsOnlyCommentaries()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.Commentaries,
            subContext: "",
            out _,
            maxResults: 10).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, result => Assert.Equal(nameof(Commentary), result.Type));
    }

    [Fact]
    public void Search_CommentariesContext_SubContextFiltersByCommentator()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.Commentaries,
            subContext: "Kathir",
            out _,
            maxResults: 10).ToList();

        Assert.Single(results);
    }

    [Fact]
    public void Search_HadithsContext_ReturnsOnlyHadiths()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.Hadiths,
            subContext: "",
            out _,
            maxResults: 10).ToList();

        Assert.NotEmpty(results);
        Assert.All(results, result => Assert.Equal(nameof(Hadith), result.Type));
    }

    [Fact]
    public void Search_HadithsContext_SubContextFiltersByCollection()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.Hadiths,
            subContext: "Bukhari",
            out _,
            maxResults: 10).ToList();

        Assert.Single(results);
    }

    [Fact]
    public void Search_WholeSite_ReturnsAllMatchingTypes()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.WholeSite,
            subContext: "",
            out _,
            maxResults: 20).ToList();

        var distinctTypes = results.Select(x => x.Type).Distinct().ToList();
        Assert.Contains(nameof(Verse), distinctTypes);
        Assert.Contains(nameof(Commentary), distinctTypes);
        Assert.Contains(nameof(Hadith), distinctTypes);
    }

    [Fact]
    public void Search_UnknownContext_ReturnsAllMatchingTypes()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: "UNKNOWN",
            subContext: "",
            out _,
            maxResults: 20).ToList();

        Assert.NotEmpty(results);
    }

    [Fact]
    public void Search_MaxResultsCap_TrimsReturnedResults()
    {
        SeedMixedDocuments();

        var results = _searchEngine.Search(
            queryString: "guidance",
            context: SearchContexts.WholeSite,
            subContext: "",
            out int total,
            maxResults: 1).ToList();

        Assert.Single(results);
        Assert.True(total >= 1);
    }

    private sealed class StubWriter : ILuceneIndexWriterProvider
    {
        private readonly IndexWriter _writer;
        public StubWriter(IndexWriter writer) => _writer = writer;
        public IndexWriter GetIndexWriter() => _writer;
    }
}
