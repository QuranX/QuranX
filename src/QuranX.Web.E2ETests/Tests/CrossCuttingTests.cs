using FluentAssertions;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class CrossCuttingTests : QuranXPageTest
{
    public CrossCuttingTests(WebHostFixture host) : base(host) { }

    [Theory]
    [InlineData("/")]
    [InlineData("/About")]
    [InlineData("/Help")]
    [InlineData("/Tafsirs")]
    [InlineData("/Hadiths")]
    public async Task SmokeUrls_ReturnHtmlContentType(string path)
    {
        var response = await GotoAsync(path);
        (await StatusAsync(response)).Should().Be(200);

        var headers = response!.Headers;
        headers.Should().ContainKey("content-type");
        headers["content-type"].Should().Contain("text/html");
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/1.1")]
    [InlineData("/About")]
    [InlineData("/Tafsirs")]
    public async Task Returning200_HasTitleAndCanonical(string path)
    {
        var response = await GotoAsync(path);
        (await StatusAsync(response)).Should().Be(200);

        var title = await Page.TitleAsync();
        title.Should().NotBeNullOrWhiteSpace();

        var canonicalCount = await Page.Locator("link[rel=canonical]").CountAsync();
        canonicalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task RepeatedRequest_ToSamePath_ProducesSameContent()
    {
        var first = await GotoAsync("/1.1");
        (await StatusAsync(first)).Should().Be(200);
        var bodyA = await Page.Locator("body").InnerTextAsync();

        var second = await GotoAsync("/1.1");
        (await StatusAsync(second)).Should().Be(200);
        var bodyB = await Page.Locator("body").InnerTextAsync();

        bodyB.Should().Be(bodyA);
    }
}
