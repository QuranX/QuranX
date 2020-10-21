using System.Web.Hosting;
using QuranX.Persistence.Services;
using QuranX.Web.Factories;
using Unity;

namespace QuranX.Web.Services
{
	public static class Registration
	{
		public static void Register(IUnityContainer container)
		{
			string dataPath = HostingEnvironment.MapPath("~/App_Data");
			var settings = new Settings(dataPath);
			container.RegisterInstance<ISettings>(settings);
			RegisterServices(container);
		}

		private static void RegisterServices(IUnityContainer container)
		{
			container.RegisterSingleton<ICommentariesForVerseFactory, CommentariesForVerseFactory>();
			container.RegisterSingleton<ISelectChapterAndVerseFactory, SelectChapterAndVerseFactory>();
			container.RegisterSingleton<IHadithViewModelFactory, HadithViewModelFactory>();
			container.RegisterSingleton<ISearchResultWithLinkFactory, SearchResultWithLinkMapper>();
		}

	}
}