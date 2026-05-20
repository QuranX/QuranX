namespace QuranX.Web.E2ETests.Infrastructure;

public sealed class WebHostFixture : IAsyncLifetime
{
    private QuranXWebApplicationFactory? _factory;

    public string BaseAddress { get; private set; } = string.Empty;

    public Task InitializeAsync()
    {
        _factory = new QuranXWebApplicationFactory();
        BaseAddress = _factory.ServerAddress;
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _factory?.Dispose();
        _factory = null;
        return Task.CompletedTask;
    }
}
