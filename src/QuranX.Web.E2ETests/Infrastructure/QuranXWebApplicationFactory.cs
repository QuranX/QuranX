using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace QuranX.Web.E2ETests.Infrastructure;

public sealed class QuranXWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? _kestrelHost;

    public string ServerAddress
    {
        get
        {
            EnsureServer();
            var feature = _kestrelHost!.Services.GetRequiredService<IServer>()
                .Features.Get<IServerAddressesFeature>()
                ?? throw new InvalidOperationException("Kestrel did not expose IServerAddressesFeature.");
            return feature.Addresses.First().TrimEnd('/');
        }
    }

    private void EnsureServer()
    {
        if (_kestrelHost is not null) return;
        // Touch CreateHost via the base CreateClient path so the host is built.
        using var _ = CreateDefaultClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(WebProjectLocator.Locate());
        builder.UseEnvironment("Production");
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Build the in-memory TestServer host that WebApplicationFactory expects.
        var testHost = builder.Build();

        // Build a parallel Kestrel host bound to a dynamic port so Playwright
        // can hit a real HTTP endpoint. Reuses the same configuration.
        builder.ConfigureWebHost(web =>
        {
            web.UseKestrel();
            web.UseUrls("http://127.0.0.1:0");
        });
        _kestrelHost = builder.Build();
        _kestrelHost.Start();

        testHost.Start();
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _kestrelHost is not null)
        {
            _kestrelHost.StopAsync().GetAwaiter().GetResult();
            _kestrelHost.Dispose();
            _kestrelHost = null;
        }
        base.Dispose(disposing);
    }
}
