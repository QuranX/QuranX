using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using QuranX.Persistence.Services;
using QuranX.Web.Factories;

namespace QuranX.Web.Services
{
	public static class Registration
	{
		public static void Register(IWebHostEnvironment environment, IServiceCollection services)
		{
			string dataPath = Path.Combine(environment.ContentRootPath, "App_Data");
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