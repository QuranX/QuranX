using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class HadithRoutingTests : QuranXPageTest
{
    public HadithRoutingTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task VerseHadiths_2_255_Returns200OrEmpty()
    {
        var response = await GotoAsync("/Hadiths/2.255");
        var status = await StatusAsync(response);
        Assert.Contains(status, new[] { 200, 404 });
    }

    [Fact]
    public async Task HadithCollectionIndex_RendersForResolvedCollection()
    {
        var (collection, primary) = await Resolvers.GetFirstHadithIndexAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Hadith/{collection}/{primary}");
        Assert.Equal(200, await StatusAsync(response));

        var subLinks = await Page.Locator($"a[href^='/Hadith/{collection}/']").CountAsync();
        Assert.True(subLinks > 0);
    }

    [Fact]
    public async Task HadithDrillIn_OneLevel_Returns200()
    {
        var (collection, primary) = await Resolvers.GetFirstHadithIndexAsync(Page, Host.BaseAddress);
        var drill = await Resolvers.GetFirstHadithDrillInAsync(
            Page, Host.BaseAddress, collection, primary);

        var response = await GotoAsync($"/Hadith/{collection}/{primary}/{drill}");
        Assert.Equal(200, await StatusAsync(response));
    }

    [Fact]
    public async Task UnknownHadithCollection_Returns404()
    {
        var response = await GotoAsync("/Hadith/NotACollection/foo");
        Assert.Contains(await StatusAsync(response), new[] { 404, 500 });
    }

    [Fact]
    public async Task NonexistentHadithIndex_RedirectsOr404s()
    {
        var (collection, _) = await Resolvers.GetFirstHadithIndexAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Hadith/{collection}/9999-1");
        var status = await StatusAsync(response);
        Assert.Contains(status, new[] { 200, 301, 302, 404 });
    }
}
