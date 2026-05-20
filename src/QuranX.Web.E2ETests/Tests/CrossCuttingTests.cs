using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
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
        Assert.Equal(200, await StatusAsync(response));

        var headers = response!.Headers;
        Assert.True(headers.ContainsKey("content-type"));
        Assert.Contains("text/html", headers["content-type"]);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/1.1")]
    [InlineData("/About")]
    [InlineData("/Tafsirs")]
    public async Task Returning200_HasTitleAndCanonical(string path)
    {
        var response = await GotoAsync(path);
        Assert.Equal(200, await StatusAsync(response));

        var title = await Page.TitleAsync();
        Assert.False(string.IsNullOrWhiteSpace(title));

        var canonicalCount = await Page.Locator("link[rel=canonical]").CountAsync();
        Assert.True(canonicalCount > 0);
    }

    [Fact]
    public async Task RepeatedRequest_ToSamePath_ProducesSameContent()
    {
        var first = await GotoAsync("/1.1");
        Assert.Equal(200, await StatusAsync(first));
        var bodyA = await Page.Locator("body").InnerTextAsync();

        var second = await GotoAsync("/1.1");
        Assert.Equal(200, await StatusAsync(second));
        var bodyB = await Page.Locator("body").InnerTextAsync();

        Assert.Equal(bodyA, bodyB);
    }
}
