using Microsoft.Extensions.DependencyInjection;
using QuranX.Persistence.Services;
using QuranX.Web.Factories;

namespace QuranX.Web.Services
{
	public static class Registration
	{
		public static void Register(IServiceCollection services)
		{
			// TODO: Get correct data path HostingEnvironment.MapPath("~/App_Data");
			string dataPath = "";
			var settings = new Settings(dataPath);
			services.AddSingleton<ISettings>(settings);
			RegisterServices(services);
		}

		private static void RegisterServices(IServiceCollection services)
		{
			services.AddSingleton<ICommentariesForVerseFactory, CommentariesForVerseFactory>();
			services.AddSingleton<ISelectChapterAndVerseFactory, SelectChapterAndVerseFactory>();
			services.AddSingleton<IHadithViewModelFactory, HadithViewModelFactory>();
			services.AddSingleton<ISearchResultWithLinkFactory, SearchResultWithLinkMapper>();
		}

	}
}