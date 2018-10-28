using System.Web.Hosting;
using System.Web.Mvc;
using QuranX.Persistence.Services;
using QuranX.Web.Factories;
using Unity;
using Unity.Mvc5;

namespace QuranX.Web
{
	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			IUnityContainer container = new UnityContainer();
			RegisterSettings(container);
			RegisterServices(container);
			Persistence.Services.Registration.Register(container);
			DependencyResolver.SetResolver(new UnityDependencyResolver(container));
		}

		private static void RegisterSettings(IUnityContainer container)
		{
			string dataPath = HostingEnvironment.MapPath("~/App_Data");
			var settings = new Settings(dataPath);
			container.RegisterInstance<ISettings>(settings);
		}

		private static void RegisterServices(IUnityContainer container)
		{
			container.RegisterSingleton<ICommentariesForVerseFactory, CommentariesForVerseFactory>();
			container.RegisterSingleton<ISelectChapterAndVerseFactory, SelectChapterAndVerseFactory>();
			container.RegisterSingleton<IHadithViewModelFactory, HadithViewModelFactory>();
		}
	}
}