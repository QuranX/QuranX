using QuranX.Persistence.Services.Lucene;
using Unity;

namespace QuranX.Persistence.Services
{
	public static class Registration
	{
		public static void Register(IUnityContainer container)
		{
			container.RegisterSingleton<IChapterRepository, ChapterRepository>();
			container.RegisterSingleton<ILuceneDirectoryProvider, LuceneDirectoryProvider>();
		}
	}
}
