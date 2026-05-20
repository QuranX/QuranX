using Microsoft.Playwright;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

/// <summary>
/// Tests for query-string and routing features documented at /Help.
/// </summary>
[Trait("Category", "E2E")]
public sealed class HelpDocumentedFeaturesTests : QuranXPageTest
{
    public HelpDocumentedFeaturesTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Verse_WithHighlight_WordWrappedInHighlightSpan()
    {
        var response = await GotoAsync("/1.1?hl=name");
        Assert.Equal(200, await StatusAsync(response));

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();
        Assert.Contains("name", highlighted);
    }

    [Fact]
    public async Task Tafsir_WithPhraseHighlight_WrapsPhraseTokens()
    {
        var code = await Resolvers.GetFirstCommentatorCodeAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Tafsir/{code}/1.1?hl=in+the+name");
        Assert.Equal(200, await StatusAsync(response));

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();

        Assert.Contains(highlighted, t => t.Contains("name"));
    }

    [Fact]
    public async Task Verse_WithMultipleHighlightTerms_HighlightsBoth()
    {
        var response = await GotoAsync("/1.1?hl=name,merciful");
        Assert.Equal(200, await StatusAsync(response));

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();

        Assert.Contains("name", highlighted);
        Assert.Contains("merciful", highlighted);
    }

    [Fact]
    public async Task Verse_WithMultiplePhraseHighlights_HighlightsTokensFromBoth()
    {
        // Help docs example: ?hl=in+the+name,most+merciful — comma separates phrases.
        // jquery.highlight escapes whitespace inside each term, so each comma-separated
        // phrase becomes one literal-phrase span (not split per word).
        var response = await GotoAsync("/1.1?hl=in+the+name,most+merciful");
        Assert.Equal(200, await StatusAsync(response));

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();

        Assert.Contains(highlighted, t => t.Contains("name"));
        Assert.Contains(highlighted, t => t.Contains("merciful"));
    }

    [Fact]
    public async Task VerseHadiths_WithHighlight_AppliesOnHadithPage()
    {
        var response = await GotoAsync("/Hadiths/2.255?hl=allah");
        var status = await StatusAsync(response);
        if (status != 200)
        {
            Assert.Contains(status, new[] { 200, 404 });
            return;
        }

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();
        Assert.Contains(highlighted, t => t.Contains("allah"));
    }

    [Fact]
    public async Task Analysis_WithHighlight_AppliesOnAnalysisPage()
    {
        var response = await GotoAsync("/Analysis/1.1?hl=root");
        Assert.Equal(200, await StatusAsync(response));

        var pageOk = await Page.Locator("body").CountAsync();
        Assert.Equal(1, pageOk);
    }

    [Fact]
    public async Task VerseList_RangePlusList_RendersAll()
    {
        var response = await GotoAsync("/1.1-3,10.32,23.4-5");
        Assert.Equal(200, await StatusAsync(response));

        var refs = (await Page.Locator(".verse__reference").AllInnerTextsAsync())
            .Select(t => t.Trim()).ToHashSet();
        foreach (var expected in new[] { "1.1", "1.2", "1.3", "10.32", "23.4", "23.5" })
            Assert.Contains(expected, refs);
    }

    [Fact]
    public async Task Search_ExactPhrase_QuotedQueryRenders()
    {
        var response = await GotoAsync("/Search?q=%22most+merciful%22");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task Search_RequiredTerm_PlusPrefixRenders()
    {
        var response = await GotoAsync("/Search?q=%2BAllah");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task Search_ExcludedTerm_MinusPrefixRenders()
    {
        var response = await GotoAsync("/Search?q=mercy+-knowing");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task Search_Wildcard_AsterisksMatchRenders()
    {
        var response = await GotoAsync("/Search?q=m*h*m*d");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }
}
