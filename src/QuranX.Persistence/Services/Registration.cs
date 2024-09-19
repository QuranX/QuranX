using Microsoft.Extensions.DependencyInjection;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Persistence.Services
{
	public static class Registration
	{
		public static void Register(IServiceCollection services)
		{
			RegisterLucene(services);
			services.AddSingleton<IChapterRepository, ChapterRepository>();
			services.AddSingleton<ICommentaryRepository, CommentaryRepository>();
			services.AddSingleton<ICommentaryWriteRepository, CommentaryWriteRepository>();
			services.AddSingleton<ICommentatorRepository, CommentatorRepository>();
			services.AddSingleton<ICommentatorWriteRepository, CommentatorWriteRepository>();
			services.AddSingleton<IDictionaryRepository, DictionaryRepository>();
			services.AddSingleton<IDictionaryWriteRepository, DictionaryWriteRepository>();
			services.AddSingleton<IDictionaryEntryRepository, DictionaryEntryRepository>();
			services.AddSingleton<IDictionaryEntryWriteRepository, DictionaryEntryWriteRepository>();
			services.AddSingleton<IHadithCollectionRepository, HadithCollectionRepository>();
			services.AddSingleton<IHadithCollectionWriteRepository, HadithCollectionWriteRepository>();
			services.AddSingleton<IHadithRepository, HadithRepository>();
			services.AddSingleton<IHadithWriteRepository, HadithWriteRepository>();
			services.AddSingleton<IVerseRepository, VerseRepository>();
			services.AddSingleton<IVerseWriteRepository, VerseWriteRepository>();
			services.AddSingleton<IVerseAnalysisRepository, VerseAnalysisRepository>();
			services.AddSingleton<IVerseAnalysisWriteRepository, VerseAnalysisWriteRepository>();
			services.AddSingleton<ISearchEngine, SearchEngine>();
		}

		private static void RegisterLucene(IServiceCollection services)
		{
			services.AddSingleton<ILuceneDirectoryProvider, LuceneDirectoryProvider>();
			services.AddSingleton<ILuceneAnalyzerProvider, LuceneAnalyzerProvider>();
			services.AddSingleton<ILuceneIndexWriterProvider, LuceneIndexWriterProvider>();
			services.AddSingleton<ILuceneIndexReaderProvider, LuceneIndexReaderProvider>();
			services.AddSingleton<ILuceneIndexSearcherProvider, LuceneIndexSearcherProvider>();
		}
	}
}
