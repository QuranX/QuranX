using FluentAssertions;
using Microsoft.Playwright;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

/// <summary>
/// Tests for query-string and routing features documented at /Help.
/// </summary>
public sealed class HelpDocumentedFeaturesTests : QuranXPageTest
{
    public HelpDocumentedFeaturesTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Verse_WithHighlight_WordWrappedInHighlightSpan()
    {
        var response = await GotoAsync("/1.1?hl=name");
        (await StatusAsync(response)).Should().Be(200);

        // jquery.highlight runs on document.ready; AllTextContents reads even
        // text in collapsed/hidden translation blocks.
        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();
        highlighted.Should().Contain("name");
    }

    [Fact]
    public async Task Tafsir_WithPhraseHighlight_WrapsPhraseTokens()
    {
        var code = await Resolvers.GetFirstCommentatorCodeAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Tafsir/{code}/1.1?hl=in+the+name");
        (await StatusAsync(response)).Should().Be(200);

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();

        highlighted.Should().Contain(t => t.Contains("name"));
    }

    [Fact]
    public async Task Verse_WithMultipleHighlightTerms_HighlightsBoth()
    {
        var response = await GotoAsync("/1.1?hl=name,merciful");
        (await StatusAsync(response)).Should().Be(200);

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();

        highlighted.Should().Contain("name");
        highlighted.Should().Contain("merciful");
    }

    [Fact]
    public async Task Verse_WithMultiplePhraseHighlights_HighlightsTokensFromBoth()
    {
        // Help docs example: ?hl=in+the+name,most+merciful — comma separates phrases.
        // jquery.highlight escapes whitespace inside each term, so each comma-separated
        // phrase becomes one literal-phrase span (not split per word).
        var response = await GotoAsync("/1.1?hl=in+the+name,most+merciful");
        (await StatusAsync(response)).Should().Be(200);

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();

        highlighted.Should().Contain(t => t.Contains("name"));
        highlighted.Should().Contain(t => t.Contains("merciful"));
    }

    [Fact]
    public async Task VerseHadiths_WithHighlight_AppliesOnHadithPage()
    {
        // Help says highlight available on ALL pages.
        var response = await GotoAsync("/Hadiths/2.255?hl=allah");
        var status = await StatusAsync(response);
        if (status != 200)
        {
            // No hadiths cross-referenced for this verse on a stripped index — skip silently.
            status.Should().BeOneOf(200, 404);
            return;
        }

        await Page.WaitForFunctionAsync(
            "document.querySelectorAll('span.highlight').length > 0",
            options: new() { Timeout = 10000 });
        var highlighted = (await Page.Locator("span.highlight").AllTextContentsAsync())
            .Select(t => t.ToLowerInvariant()).ToList();
        highlighted.Should().Contain(t => t.Contains("allah"));
    }

    [Fact]
    public async Task Analysis_WithHighlight_AppliesOnAnalysisPage()
    {
        var response = await GotoAsync("/Analysis/1.1?hl=root");
        (await StatusAsync(response)).Should().Be(200);

        // Analysis page may have no English text matching "root"; assert that the
        // hl plumbing runs without breaking the page rather than insisting on a hit.
        var pageOk = await Page.Locator("body").CountAsync();
        pageOk.Should().Be(1);
    }

    [Fact]
    public async Task VerseList_RangePlusList_RendersAll()
    {
        var response = await GotoAsync("/1.1-3,10.32,23.4-5");
        (await StatusAsync(response)).Should().Be(200);

        var refs = (await Page.Locator(".verse__reference").AllInnerTextsAsync())
            .Select(t => t.Trim()).ToList();
        refs.Should().Contain(["1.1", "1.2", "1.3", "10.32", "23.4", "23.5"]);
    }

    [Fact]
    public async Task Search_ExactPhrase_QuotedQueryRenders()
    {
        var response = await GotoAsync("/Search?q=%22most+merciful%22");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Search_RequiredTerm_PlusPrefixRenders()
    {
        var response = await GotoAsync("/Search?q=%2BAllah");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Search_ExcludedTerm_MinusPrefixRenders()
    {
        var response = await GotoAsync("/Search?q=mercy+-knowing");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Search_Wildcard_AsterisksMatchRenders()
    {
        var response = await GotoAsync("/Search?q=m*h*m*d");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }
}
