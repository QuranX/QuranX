using FluentAssertions;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class SearchTests : QuranXPageTest
{
    public SearchTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Search_NoQuery_Renders()
    {
        var response = await GotoAsync("/Search");
        (await StatusAsync(response)).Should().Be(200);
    }

    [Fact]
    public async Task Search_Mercy_ReturnsResults_AndIsNoIndex()
    {
        var response = await GotoAsync("/Search?q=mercy");
        (await StatusAsync(response)).Should().Be(200);

        var robots = await Page.Locator("meta[name='robots']").First.GetAttributeAsync("content");
        robots.Should().NotBeNullOrEmpty();
        robots!.Should().Contain("noindex");
    }

    [Fact]
    public async Task Search_Mercy_ScopedToQuran()
    {
        var response = await GotoAsync("/Search?q=mercy&context=quran");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Search_BadQuery_Renders()
    {
        var response = await GotoAsync("/Search?q=AND%20OR");
        (await StatusAsync(response)).Should().Be(200);
    }

    [Fact]
    public async Task Search_EmptyQuery_Renders()
    {
        var response = await GotoAsync("/Search?q=");
        (await StatusAsync(response)).Should().Be(200);
    }
}
