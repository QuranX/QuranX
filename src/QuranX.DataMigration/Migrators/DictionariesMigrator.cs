namespace QuranX.DataMigration.Migrators;

public interface IDictionariesMigrator
{
    void Migrate();
}

public class DictionariesMigrator : IDictionariesMigrator
{
    private readonly ILisaanDictionaryMigrator LisaanDictionaryMigrator;

    public DictionariesMigrator(ILisaanDictionaryMigrator lisaanDictionaryMigrator)
    {
        LisaanDictionaryMigrator = lisaanDictionaryMigrator;
    }

    public void Migrate()
    {
        LisaanDictionaryMigrator.Migrate();
    }
}
