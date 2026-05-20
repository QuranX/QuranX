using FluentAssertions;
using QuranX.Web.E2ETests.Infrastructure;

namespace QuranX.Web.E2ETests.Tests;

public sealed class DictionaryTests : QuranXPageTest
{
    public DictionaryTests(WebHostFixture host) : base(host) { }

    [Fact]
    public async Task DictionariesIndex_Renders()
    {
        var response = await GotoAsync("/Dictionaries");
        (await StatusAsync(response)).Should().Be(200);

        var roots = await Page.Locator("a[href^='/Dictionaries/']").CountAsync();
        roots.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DictionariesByRoot_RendersEntries()
    {
        var root = await Resolvers.FindDictionaryRootWithEntriesAsync(Page, Host.BaseAddress);

        var response = await GotoAsync($"/Dictionaries/{Uri.EscapeDataString(root)}");
        (await StatusAsync(response)).Should().Be(200);

        var entries = await Page.Locator(".dictionary-entries__entry").CountAsync();
        entries.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task DictionaryEntry_Renders()
    {
        var root = await Resolvers.FindDictionaryRootWithEntriesAsync(Page, Host.BaseAddress);
        var (code, word) = await Resolvers.GetFirstDictionaryEntryAsync(Page, Host.BaseAddress, root);

        var response = await GotoAsync($"/Dictionary/{code}/{Uri.EscapeDataString(word)}");
        (await StatusAsync(response)).Should().Be(200);

        var body = await Page.Locator("body").InnerTextAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UnknownDictionary_Returns404()
    {
        var response = await GotoAsync("/Dictionary/NotADict/foo");
        (await StatusAsync(response)).Should().BeOneOf(404, 500);
    }
}
