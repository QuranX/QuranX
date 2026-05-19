namespace QuranX.DataMigration.Migrators;

public sealed record class MigratorSet(
    ICorpusMigrator Corpus,
    IQuranMigrator Quran,
    ICommentaryMigrator Commentary,
    IHadithMigrator Hadith,
    IDictionariesMigrator Dictionaries);
