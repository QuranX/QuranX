using FluentAssertions;
using Microsoft.Playwright;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class SmokeTests : QuranXPageTest
{
    public SmokeTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Home_Returns200_AndShowsQuranHeading()
    {
        var response = await GotoAsync("/");
        (await StatusAsync(response)).Should().Be(200);

        var h1Text = await Page.Locator("h1").First.InnerTextAsync();
        h1Text.Should().Contain("Qur'an");

        var canonical = await Page.Locator("link[rel=canonical]").CountAsync();
        canonical.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task About_Returns200_AndTitleContainsAbout()
    {
        var response = await GotoAsync("/About");
        (await StatusAsync(response)).Should().Be(200);

        var title = await Page.TitleAsync();
        title.Should().Contain("About", because: $"actual title was \"{title}\"");
    }

    [Fact]
    public async Task Help_Returns200_WithCanonical()
    {
        var response = await GotoAsync("/Help");
        (await StatusAsync(response)).Should().Be(200);

        var canonicalHref = await Page.Locator("link[rel=canonical]").First.GetAttributeAsync("href");
        canonicalHref.Should().NotBeNullOrEmpty();
        canonicalHref!.Should().EndWith("/Help");
    }

    [Fact]
    public async Task Tafsirs_ListsCommentators()
    {
        var response = await GotoAsync("/Tafsirs");
        (await StatusAsync(response)).Should().Be(200);

        var count = await Page.Locator("a[href^='/Tafsir/']").CountAsync();
        count.Should().BeGreaterOrEqualTo(5);
    }

    [Fact]
    public async Task Hadiths_ListsCollections()
    {
        var response = await GotoAsync("/Hadiths");
        (await StatusAsync(response)).Should().Be(200);

        var count = await Page.Locator("a[href^='/Hadith/']").CountAsync();
        count.Should().BeGreaterOrEqualTo(5);
    }
}
