namespace QuranX.Web.E2ETests.Infrastructure;

internal static class WebProjectLocator
{
    public static string Locate()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "QuranX.Web");
            if (Directory.Exists(Path.Combine(candidate, "App_Data")))
                return candidate;
            dir = dir.Parent;
        }
        throw new InvalidOperationException(
            $"Could not locate QuranX.Web with App_Data starting from {AppContext.BaseDirectory}. " +
            "The Lucene index in App_Data must be built (run QuranX.Web once or QuranX.DataMigration).");
    }
}
