using QuranX.Persistence.Services;

namespace QuranX.Persistence.Tests.Services;

[Trait("Category", "Unit")]
public sealed class SettingsTests
{
    [Fact]
    public void Constructor_StoresDataPath()
    {
        Assert.Equal("C:/data", new Settings("C:/data").DataPath);
    }

    [Fact]
    public void DataPath_IsAvailableViaInterface()
    {
        ISettings settings = new Settings("X");
        Assert.Equal("X", settings.DataPath);
    }
}
