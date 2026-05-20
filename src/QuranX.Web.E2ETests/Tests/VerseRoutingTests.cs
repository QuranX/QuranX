using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class VerseRoutingTests : QuranXPageTest
{
    public VerseRoutingTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Verse_1_1_Renders()
    {
        var response = await GotoAsync("/1.1");
        Assert.Equal(200, await StatusAsync(response));

        var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
        Assert.Contains(refs, t => t.Trim() == "1.1");
    }

    [Fact]
    public async Task Verse_2_255_RendersArabic()
    {
        var response = await GotoAsync("/2.255");
        Assert.Equal(200, await StatusAsync(response));

        var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
        Assert.Contains(refs, t => t.Trim() == "2.255");

        var arabic = await Page.Locator("dd.arabic").First.InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(arabic));
    }

    [Fact]
    public async Task VerseRange_2_3to5_RendersAllThree()
    {
        var response = await GotoAsync("/2.3-5");
        Assert.Equal(200, await StatusAsync(response));

        var refs = (await Page.Locator(".verse__reference").AllInnerTextsAsync())
            .Select(t => t.Trim()).ToHashSet();
        Assert.Contains("2.3", refs);
        Assert.Contains("2.4", refs);
        Assert.Contains("2.5", refs);
    }

    [Fact]
    public async Task MultiVerse_1_1and2_2_BothRender()
    {
        var response = await GotoAsync("/1.1,2.2");
        Assert.Equal(200, await StatusAsync(response));

        var refs = (await Page.Locator(".verse__reference").AllInnerTextsAsync())
            .Select(t => t.Trim()).ToHashSet();
        Assert.Contains("1.1", refs);
        Assert.Contains("2.2", refs);
    }

    [Fact]
    public async Task LastVerse_114_6_Renders()
    {
        var response = await GotoAsync("/114.6");
        Assert.Equal(200, await StatusAsync(response));

        var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
        Assert.Contains(refs, t => t.Trim() == "114.6");
    }

    [Fact]
    public async Task InvalidChapter_999_1_DoesNotReturn200WithVerseContent()
    {
        var response = await GotoAsync("/999.1");
        var status = await StatusAsync(response);

        if (status == 200)
        {
            var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
            Assert.DoesNotContain(refs, t => t.Trim().StartsWith("999."));
        }
        else
        {
            Assert.Contains(status, new[] { 404, 500 });
        }
    }

    [Fact]
    public async Task Verse_WithContext_AddsFocalPointAnchor()
    {
        var response = await GotoAsync("/1.1?context=2");
        Assert.Equal(200, await StatusAsync(response));

        var focal = await Page.Locator("a[name='focal-point']").CountAsync();
        Assert.True(focal > 0);
    }

    [Fact]
    public async Task MalformedRoute_abc_def_Returns404()
    {
        var response = await GotoAsync("/abc.def");
        Assert.Equal(404, await StatusAsync(response));
    }
}
