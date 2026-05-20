using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace QuranX.Web.E2ETests.Infrastructure;

internal static class Resolvers
{
    public static async Task<string> GetFirstCommentatorCodeAsync(IPage page, string baseAddress)
    {
        await page.GotoAsync(baseAddress + "/Tafsirs");
        var href = await page.Locator("a[href^='/Tafsir/']").First.GetAttributeAsync("href")
            ?? throw new InvalidOperationException("No /Tafsir/<code>/ link found on /Tafsirs.");
        var match = Regex.Match(href, "^/Tafsir/([^/]+)/");
        if (!match.Success)
            throw new InvalidOperationException($"Unexpected commentator href: {href}");
        return match.Groups[1].Value;
    }

    public static async Task<(string Collection, string PrimaryReference)> GetFirstHadithIndexAsync(
        IPage page, string baseAddress)
    {
        await page.GotoAsync(baseAddress + "/Hadiths");
        var href = await page.Locator("a[href^='/Hadith/']").First.GetAttributeAsync("href")
            ?? throw new InvalidOperationException("No /Hadith/<code>/<ref> link found on /Hadiths.");
        var parts = href.TrimStart('/').Split('/');
        if (parts.Length < 3)
            throw new InvalidOperationException($"Unexpected hadith index href: {href}");
        return (parts[1], parts[2]);
    }

    public static async Task<string> GetFirstHadithDrillInAsync(
        IPage page,
        string baseAddress,
        string collectionCode,
        string primaryReferenceCode)
    {
        await page.GotoAsync($"{baseAddress}/Hadith/{collectionCode}/{primaryReferenceCode}");
        var hrefs = await page.Locator($"a[href^='/Hadith/{collectionCode}/']").EvaluateAllAsync<string[]>(
            "els => els.map(e => e.getAttribute('href'))");
        foreach (var href in hrefs)
        {
            var parts = href.TrimStart('/').Split('/');
            // /Hadith/Col/PrimaryRef/Drill1[/Drill2[/Drill3]]
            if (parts.Length >= 4 && parts[2] == primaryReferenceCode) return parts[3];
        }
        throw new InvalidOperationException(
            $"No drill-in reference found for /Hadith/{collectionCode}/{primaryReferenceCode}.");
    }

    public static async Task<string> GetFirstDictionaryRootAsync(IPage page, string baseAddress)
    {
        await page.GotoAsync(baseAddress + "/Dictionaries");
        var href = await page.Locator("a[href^='/Dictionaries/']").First.GetAttributeAsync("href")
            ?? throw new InvalidOperationException("No /Dictionaries/<root> link found.");
        return Uri.UnescapeDataString(href["/Dictionaries/".Length..]);
    }

    public static async Task<string> FindDictionaryRootWithEntriesAsync(IPage page, string baseAddress)
    {
        // Dictionary entries are inline (no anchor), so detect presence via the
        // .dictionary-entries__entry div. Walk depth-first picking the first
        // child each time until entries appear.
        await page.GotoAsync(baseAddress + "/Dictionaries");
        var visited = new HashSet<string>();
        int budget = 20;
        while (budget-- > 0)
        {
            var entryCount = await page.Locator(".dictionary-entries__entry").CountAsync();
            if (entryCount > 0)
            {
                var canonical = page.Url;
                var idx = canonical.IndexOf("/Dictionaries/", StringComparison.Ordinal);
                if (idx < 0)
                    throw new InvalidOperationException($"Unexpected URL after walk: {canonical}");
                return Uri.UnescapeDataString(canonical[(idx + "/Dictionaries/".Length)..]);
            }

            var childHrefs = await page.Locator("a[href^='/Dictionaries/']").EvaluateAllAsync<string[]>(
                "els => els.map(e => e.getAttribute('href'))");
            string? next = null;
            foreach (var href in childHrefs)
            {
                var seg = href["/Dictionaries/".Length..];
                if (seg.Length == 0) continue;
                if (!visited.Add(seg)) continue;
                next = href;
                break;
            }
            if (next is null)
                throw new InvalidOperationException("Walked dictionary tree without finding entries.");
            await page.GotoAsync(baseAddress + next);
        }
        throw new InvalidOperationException("Dictionary entry walk exceeded budget.");
    }

    public static async Task<(string Code, string Word)> GetFirstDictionaryEntryAsync(
        IPage page,
        string baseAddress,
        string root)
    {
        await page.GotoAsync($"{baseAddress}/Dictionaries/{Uri.EscapeDataString(root)}");
        // Each entry block is .dictionary-entries__entry, containing a div with
        // class "dictionary__details dictionary__details--{Code}" and an
        // <h3>Entry <span class="arabic">{Word}</span></h3>.
        var entry = page.Locator(".dictionary-entries__entry").First;
        if (await entry.CountAsync() == 0)
            throw new InvalidOperationException($"No entries on /Dictionaries/{root}.");

        var detailsClass = await entry.Locator("[class*='dictionary__details--']").First.GetAttributeAsync("class")
            ?? throw new InvalidOperationException("No dictionary__details-- class on entry.");
        var match = Regex.Match(detailsClass, @"dictionary__details--(\S+)");
        if (!match.Success)
            throw new InvalidOperationException($"Could not parse dictionary code from class: {detailsClass}");
        var code = match.Groups[1].Value;

        var word = (await entry.Locator("h3 span.arabic").First.InnerTextAsync()).Trim();
        if (string.IsNullOrEmpty(word))
            throw new InvalidOperationException("Entry word was empty.");

        return (code, word);
    }

    public static async Task<string> GetFirstAnalysisRootAsync(IPage page, string baseAddress)
    {
        await page.GotoAsync(baseAddress + "/Analysis/1.1");
        var href = await page.Locator("a[href^='/Analysis/Root/']").First.GetAttributeAsync("href")
            ?? throw new InvalidOperationException("No /Analysis/Root/<root> link on /Analysis/1.1.");
        return href.Substring("/Analysis/Root/".Length);
    }
}
