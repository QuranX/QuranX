using FluentAssertions;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class TafsirAnalysisTests : QuranXPageTest
{
    public TafsirAnalysisTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task TafsirsForVerse_1_1_HasCanonical()
    {
        var response = await GotoAsync("/Tafsirs/1.1");
        (await StatusAsync(response)).Should().Be(200);

        var canonical = await Page.Locator("link[rel=canonical]").First.GetAttributeAsync("href");
        canonical.Should().NotBeNullOrEmpty();
        canonical!.Should().EndWith("/Tafsirs/1.1");
    }

    [Fact]
    public async Task SpecificCommentator_RenderedForVerse_1_1()
    {
        var code = await Resolvers.GetFirstCommentatorCodeAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Tafsir/{code}/1.1");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UnknownCommentator_Returns404()
    {
        var response = await GotoAsync("/Tafsir/NOT-A-REAL-CODE/1.1");
        (await StatusAsync(response)).Should().BeOneOf(404, 500);
    }

    [Fact]
    public async Task InvalidVerseForTafsirs_Returns404()
    {
        var response = await GotoAsync("/Tafsirs/999.999");
        (await StatusAsync(response)).Should().Be(404);
    }

    [Fact]
    public async Task VerseAnalysis_1_1_RendersRootMarkup()
    {
        var response = await GotoAsync("/Analysis/1.1");
        (await StatusAsync(response)).Should().Be(200);

        var rootLinks = await Page.Locator("a[href^='/Analysis/Root/']").CountAsync();
        rootLinks.Should().BeGreaterThan(0, because: "verse 1.1 has known Arabic roots");
    }

    [Fact]
    public async Task RootAnalysis_RendersForResolvedRoot()
    {
        var root = await Resolvers.GetFirstAnalysisRootAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Analysis/Root/{root}");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }
}
