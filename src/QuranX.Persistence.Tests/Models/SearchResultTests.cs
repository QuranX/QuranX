using Lucene.Net.Documents;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Tests.Models;

[Trait("Category", "Unit")]
public sealed class SearchResultTests
{
    [Fact]
    public void Constructor_NonVerseType_KeepsAllSnippets()
    {
        var result = new SearchResult("Hadith", new Document(), new[] { "a", "b", "c" });
        Assert.Equal("Hadith", result.Type);
        Assert.Equal(new[] { "a", "b", "c" }, result.Snippets);
    }

    [Fact]
    public void Constructor_VerseType_TrimsSnippetsToOne()
    {
        var result = new SearchResult("Verse", new Document(), new[] { "a", "b", "c" });
        Assert.Single(result.Snippets);
        Assert.Equal("a", result.Snippets[0]);
    }

    [Fact]
    public void Constructor_VerseType_CaseInsensitiveMatch()
    {
        var result = new SearchResult("verse", new Document(), new[] { "a", "b" });
        Assert.Single(result.Snippets);
    }

    [Fact]
    public void Constructor_StoresDocument()
    {
        var document = new Document();
        Assert.Same(
            document,
            new SearchResult("Hadith", document, Array.Empty<string>()).Document);
    }
}
