using Lucene.Net.Index;
using QuranX.DataMigration.Services;
using QuranX.Persistence.Services;
using System.IO;

namespace QuranX.DataMigration.Migrators;

public interface IDataMigrator
{
    void Migrate();
}

public class DataMigrator : IDataMigrator
{
    private readonly ICorpusMigrator CorpusMigrator;
    private readonly IQuranMigrator QuranMigrator;
    private readonly ICommentaryMigrator CommentaryMigrator;
    private readonly IHadithMigrator HadithMigrator;
    private readonly ILuceneIndexWriterProvider IndexWriterProvider;
    private readonly IDictionariesMigrator DictionariesMigrator;
    private readonly ISitemapGenerator SitemapGenerator;
    private readonly ISettings Settings;

    public DataMigrator(
        MigratorSet migrators,
        ILuceneIndexWriterProvider indexWriterProvider,
        ISitemapGenerator sitemapGenerator,
        ISettings settings)
    {
        CorpusMigrator = migrators.Corpus;
        QuranMigrator = migrators.Quran;
        CommentaryMigrator = migrators.Commentary;
        HadithMigrator = migrators.Hadith;
        DictionariesMigrator = migrators.Dictionaries;
        IndexWriterProvider = indexWriterProvider;
        SitemapGenerator = sitemapGenerator;
        Settings = settings;
    }

    public void Migrate()
    {
        Directory.Delete(Settings.DataPath, true);
        Directory.CreateDirectory(Settings.DataPath);

        DictionariesMigrator.Migrate();
        CorpusMigrator.Migrate();
        QuranMigrator.Migrate();
        CommentaryMigrator.Migrate();
        HadithMigrator.Migrate();

        IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
        indexWriter.Commit();
        indexWriter.ForceMerge(1, doWait: true);

#if !DEBUG
        SitemapGenerator.Generate();
#endif
    }
}
