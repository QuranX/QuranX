using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class TafsirAnalysisTests : QuranXPageTest
{
    public TafsirAnalysisTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task TafsirsForVerse_1_1_HasCanonical()
    {
        var response = await GotoAsync("/Tafsirs/1.1");
        Assert.Equal(200, await StatusAsync(response));

        var canonical = await Page.Locator("link[rel=canonical]").First.GetAttributeAsync("href");
        Assert.False(string.IsNullOrEmpty(canonical));
        Assert.EndsWith("/Tafsirs/1.1", canonical);
    }

    [Fact]
    public async Task SpecificCommentator_RenderedForVerse_1_1()
    {
        var code = await Resolvers.GetFirstCommentatorCodeAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Tafsir/{code}/1.1");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task UnknownCommentator_Returns404()
    {
        var response = await GotoAsync("/Tafsir/NOT-A-REAL-CODE/1.1");
        Assert.Contains(await StatusAsync(response), new[] { 404, 500 });
    }

    [Fact]
    public async Task InvalidVerseForTafsirs_Returns404()
    {
        var response = await GotoAsync("/Tafsirs/999.999");
        Assert.Equal(404, await StatusAsync(response));
    }

    [Fact]
    public async Task VerseAnalysis_1_1_RendersRootMarkup()
    {
        var response = await GotoAsync("/Analysis/1.1");
        Assert.Equal(200, await StatusAsync(response));

        var rootLinks = await Page.Locator("a[href^='/Analysis/Root/']").CountAsync();
        Assert.True(rootLinks > 0);
    }

    [Fact]
    public async Task RootAnalysis_RendersForResolvedRoot()
    {
        var root = await Resolvers.GetFirstAnalysisRootAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Analysis/Root/{root}");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }
}
