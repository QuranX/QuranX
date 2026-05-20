using Microsoft.Playwright;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class SmokeTests : QuranXPageTest
{
    public SmokeTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Home_Returns200_AndShowsQuranHeading()
    {
        var response = await GotoAsync("/");
        Assert.Equal(200, await StatusAsync(response));

        var h1Text = await Page.Locator("h1").First.InnerTextAsync();
        Assert.Contains("Qur'an", h1Text);

        var canonical = await Page.Locator("link[rel=canonical]").CountAsync();
        Assert.True(canonical > 0);
    }

    [Fact]
    public async Task About_Returns200_AndTitleContainsAbout()
    {
        var response = await GotoAsync("/About");
        Assert.Equal(200, await StatusAsync(response));

        var title = await Page.TitleAsync();
        Assert.Contains("About", title);
    }

    [Fact]
    public async Task Help_Returns200_WithCanonical()
    {
        var response = await GotoAsync("/Help");
        Assert.Equal(200, await StatusAsync(response));

        var canonicalHref = await Page.Locator("link[rel=canonical]").First.GetAttributeAsync("href");
        Assert.False(string.IsNullOrEmpty(canonicalHref));
        Assert.EndsWith("/Help", canonicalHref);
    }

    [Fact]
    public async Task Tafsirs_ListsCommentators()
    {
        var response = await GotoAsync("/Tafsirs");
        Assert.Equal(200, await StatusAsync(response));

        var count = await Page.Locator("a[href^='/Tafsir/']").CountAsync();
        Assert.True(count >= 5);
    }

    [Fact]
    public async Task Hadiths_ListsCollections()
    {
        var response = await GotoAsync("/Hadiths");
        Assert.Equal(200, await StatusAsync(response));

        var count = await Page.Locator("a[href^='/Hadith/']").CountAsync();
        Assert.True(count >= 5);
    }
}
