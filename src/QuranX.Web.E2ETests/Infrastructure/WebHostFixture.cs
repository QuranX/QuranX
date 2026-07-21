namespace QuranX.Web.E2ETests.Infrastructure;

/// <summary>
/// Exposes the base address of the QuranX web app under test.
/// The underlying Kestrel host is a process-wide singleton: the site is
/// read-only, so a single host safely serves every test class in parallel,
/// and we avoid re-booting the app (and re-opening the Lucene index) per class.
/// Used as an <see cref="IClassFixture{T}"/> so each parallel test class gets a
/// handle, but only one host is ever started; it is torn down at process exit.
/// </summary>
public sealed class WebHostFixture : IAsyncLifetime
{
    private static QuranXWebApplicationFactory? _factory;
    private static readonly Lock SyncRoot = new Lock();

    public string BaseAddress { get; private set; } = string.Empty;

    public Task InitializeAsync()
    {
        BaseAddress = GetOrCreateFactory().ServerAddress;
        return Task.CompletedTask;
    }

    // No-op: the shared host outlives any individual class and is disposed at process exit.
    public Task DisposeAsync() => Task.CompletedTask;

    private static QuranXWebApplicationFactory GetOrCreateFactory()
    {
        if (_factory is not null) return _factory;
        lock (SyncRoot)
        {
            if (_factory is null)
            {
                var factory = new QuranXWebApplicationFactory();
                _ = factory.ServerAddress; // force the host to build and start now
                AppDomain.CurrentDomain.ProcessExit += (_, _) => factory.Dispose();
                _factory = factory;
            }
        }
        return _factory;
    }
}
