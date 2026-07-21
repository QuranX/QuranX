using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;

namespace QuranX.Web.E2ETests.Infrastructure;

public abstract class QuranXPageTest : PageTest, IClassFixture<WebHostFixture>
{
    protected WebHostFixture Host { get; }

    protected QuranXPageTest(WebHostFixture host)
    {
        Host = host;
    }

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        IgnoreHTTPSErrors = true,
        BaseURL = Host.BaseAddress,
    };

    protected Task<IResponse?> GotoAsync(string relativePath, PageGotoOptions? options = null)
    {
        if (!relativePath.StartsWith('/')) relativePath = "/" + relativePath;
        return Page.GotoAsync(Host.BaseAddress + relativePath, options);
    }

    protected static async Task<int> StatusAsync(IResponse? response)
    {
        if (response is null) throw new InvalidOperationException("No response received.");
        // Force-finish in case Playwright is still streaming.
        await response.FinishedAsync();
        return response.Status;
    }
}
