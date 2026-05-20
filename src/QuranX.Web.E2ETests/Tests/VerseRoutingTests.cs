using FluentAssertions;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class VerseRoutingTests : QuranXPageTest
{
    public VerseRoutingTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task Verse_1_1_Renders()
    {
        var response = await GotoAsync("/1.1");
        (await StatusAsync(response)).Should().Be(200);

        var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
        refs.Should().Contain(t => t.Trim() == "1.1");
    }

    [Fact]
    public async Task Verse_2_255_RendersArabic()
    {
        var response = await GotoAsync("/2.255");
        (await StatusAsync(response)).Should().Be(200);

        var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
        refs.Should().Contain(t => t.Trim() == "2.255");

        var arabic = await Page.Locator("dd.arabic").First.InnerTextAsync();
        arabic.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task VerseRange_2_3to5_RendersAllThree()
    {
        var response = await GotoAsync("/2.3-5");
        (await StatusAsync(response)).Should().Be(200);

        var refs = (await Page.Locator(".verse__reference").AllInnerTextsAsync())
            .Select(t => t.Trim()).ToList();
        refs.Should().Contain(["2.3", "2.4", "2.5"]);
    }

    [Fact]
    public async Task MultiVerse_1_1and2_2_BothRender()
    {
        var response = await GotoAsync("/1.1,2.2");
        (await StatusAsync(response)).Should().Be(200);

        var refs = (await Page.Locator(".verse__reference").AllInnerTextsAsync())
            .Select(t => t.Trim()).ToList();
        refs.Should().Contain("1.1");
        refs.Should().Contain("2.2");
    }

    [Fact]
    public async Task LastVerse_114_6_Renders()
    {
        var response = await GotoAsync("/114.6");
        (await StatusAsync(response)).Should().Be(200);

        var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
        refs.Should().Contain(t => t.Trim() == "114.6");
    }

    [Fact]
    public async Task InvalidChapter_999_1_DoesNotReturn200WithVerseContent()
    {
        var response = await GotoAsync("/999.1");
        var status = await StatusAsync(response);

        // Controller has no validation guard — expect server-side error (500)
        // via UseExceptionHandler, or 404 if routing rejects. Either is acceptable
        // as long as the bogus chapter is not silently served as 200 verse content.
        if (status == 200)
        {
            var refs = await Page.Locator(".verse__reference").AllInnerTextsAsync();
            refs.Should().NotContain(t => t.Trim().StartsWith("999."),
                because: "chapter 999 does not exist and must not be rendered as a real verse");
        }
        else
        {
            status.Should().BeOneOf(404, 500);
        }
    }

    [Fact]
    public async Task Verse_WithContext_AddsFocalPointAnchor()
    {
        var response = await GotoAsync("/1.1?context=2");
        (await StatusAsync(response)).Should().Be(200);

        var focal = await Page.Locator("a[name='focal-point']").CountAsync();
        focal.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task MalformedRoute_abc_def_Returns404()
    {
        var response = await GotoAsync("/abc.def");
        (await StatusAsync(response)).Should().Be(404);
    }
}
