using FluentAssertions;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class ErrorEdgeTests : QuranXPageTest
{
    public ErrorEdgeTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task UnknownRoute_Returns404()
    {
        var response = await GotoAsync("/this-route-does-not-exist");
        (await StatusAsync(response)).Should().Be(404);
    }

    [Fact]
    public async Task ZeroVerse_1_0_DoesNotRenderRealContent()
    {
        var response = await GotoAsync("/1.0");
        var status = await StatusAsync(response);

        if (status == 200)
        {
            var refs = await Page.Locator(".verse__reference").CountAsync();
            refs.Should().Be(0, because: "verse 0 is not a valid Quran verse");
        }
        else
        {
            status.Should().BeOneOf(404, 500);
        }
    }

    [Fact]
    public async Task TafsirsForInvalidVerse_0_0_Returns404()
    {
        var response = await GotoAsync("/Tafsirs/0.0");
        (await StatusAsync(response)).Should().Be(404);
    }
}
