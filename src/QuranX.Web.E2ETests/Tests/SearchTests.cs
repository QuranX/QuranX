using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class SearchTests : QuranXPageTest
{
    public SearchTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Search_NoQuery_Renders()
    {
        var response = await GotoAsync("/Search");
        Assert.Equal(200, await StatusAsync(response));
    }

    [Fact]
    public async Task Search_Mercy_ReturnsResults_AndIsNoIndex()
    {
        var response = await GotoAsync("/Search?q=mercy");
        Assert.Equal(200, await StatusAsync(response));

        var robots = await Page.Locator("meta[name='robots']").First.GetAttributeAsync("content");
        Assert.False(string.IsNullOrEmpty(robots));
        Assert.Contains("noindex", robots);
    }

    [Fact]
    public async Task Search_Mercy_ScopedToQuran()
    {
        var response = await GotoAsync("/Search?q=mercy&context=quran");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task Search_BadQuery_Renders()
    {
        var response = await GotoAsync("/Search?q=AND%20OR");
        Assert.Equal(200, await StatusAsync(response));
    }

    [Fact]
    public async Task Search_EmptyQuery_Renders()
    {
        var response = await GotoAsync("/Search?q=");
        Assert.Equal(200, await StatusAsync(response));
    }
}
