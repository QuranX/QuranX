using QuranX.Persistence.Services.Repositories;

namespace QuranX.DataMigration.Services;

public sealed record class SitemapRepositories(
    IVerseRepository Verse,
    IDictionaryRepository Dictionary,
    IDictionaryEntryRepository DictionaryEntry,
    ICommentatorRepository Commentator,
    ICommentaryRepository Commentary,
    IVerseAnalysisRepository VerseAnalysis,
    IHadithRepository Hadith,
    IHadithCollectionRepository HadithCollection);
