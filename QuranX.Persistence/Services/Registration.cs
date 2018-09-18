using QuranX.Persistence.Services.Repositories;
using Unity;

namespace QuranX.Persistence.Services
{
	public static class Registration
	{
		public static void Register(IUnityContainer container)
		{
			RegisterLucene(container);
			container.RegisterSingleton<IChapterRepository, ChapterRepository>();
			container.RegisterSingleton<IVerseRepository, VerseRepository>();
			container.RegisterSingleton<IVerseWriteRepository, VerseWriteRepository>();
		}

		private static void RegisterLucene(IUnityContainer container)
		{
			container.RegisterSingleton<ILuceneDirectoryProvider, LuceneDirectoryProvider>();
			container.RegisterSingleton<ILuceneAnalyzerProvider, LuceneAnalyzerProvider>();
			container.RegisterSingleton<ILuceneIndexWriterProvider, LuceneIndexWriterProvider>();
			container.RegisterSingleton<ILuceneIndexReaderProvider, LuceneIndexReaderProvider>();
		}
	}
}
