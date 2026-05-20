using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class ErrorEdgeTests : QuranXPageTest
{
    public ErrorEdgeTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task UnknownRoute_Returns404()
    {
        var response = await GotoAsync("/this-route-does-not-exist");
        Assert.Equal(404, await StatusAsync(response));
    }

    [Fact]
    public async Task ZeroVerse_1_0_DoesNotRenderRealContent()
    {
        var response = await GotoAsync("/1.0");
        var status = await StatusAsync(response);

        if (status == 200)
        {
            var refs = await Page.Locator(".verse__reference").CountAsync();
            Assert.Equal(0, refs);
        }
        else
        {
            Assert.Contains(status, new[] { 404, 500 });
        }
    }

    [Fact]
    public async Task TafsirsForInvalidVerse_0_0_Returns404()
    {
        var response = await GotoAsync("/Tafsirs/0.0");
        Assert.Equal(404, await StatusAsync(response));
    }
}
