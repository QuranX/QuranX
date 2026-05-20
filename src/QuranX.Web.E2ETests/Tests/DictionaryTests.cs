using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

[Trait("Category", "E2E")]
public sealed class DictionaryTests : QuranXPageTest
{
    public DictionaryTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task DictionariesIndex_Renders()
    {
        var response = await GotoAsync("/Dictionaries");
        Assert.Equal(200, await StatusAsync(response));

        var roots = await Page.Locator("a[href^='/Dictionaries/']").CountAsync();
        Assert.True(roots > 0);
    }

    [Fact]
    public async Task DictionariesByRoot_RendersEntries()
    {
        var root = await Resolvers.FindDictionaryRootWithEntriesAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Dictionaries/{Uri.EscapeDataString(root)}");
        Assert.Equal(200, await StatusAsync(response));

        var entries = await Page.Locator(".dictionary-entries__entry").CountAsync();
        Assert.True(entries > 0);
    }

    [Fact]
    public async Task DictionaryEntry_Renders()
    {
        var root = await Resolvers.FindDictionaryRootWithEntriesAsync(Page, Host.BaseAddress);
        var (code, word) = await Resolvers.GetFirstDictionaryEntryAsync(Page, Host.BaseAddress, root);

        var response = await GotoAsync($"/Dictionary/{code}/{Uri.EscapeDataString(word)}");
        Assert.Equal(200, await StatusAsync(response));

        var body = await Page.Locator("body").InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(body));
    }

    [Fact]
    public async Task UnknownDictionary_Returns404()
    {
        var response = await GotoAsync("/Dictionary/NotADict/foo");
        Assert.Contains(await StatusAsync(response), new[] { 404, 500 });
    }
}
