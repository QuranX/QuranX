using System;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using QuranX.Persistence.Services;
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
			Persistence.Services.Registration.Register(container);

			// register all your components with the container here
			// it is NOT necessary to register your controllers

			// e.g. container.RegisterType<ITestService, TestService>();

			DependencyResolver.SetResolver(new UnityDependencyResolver(container));
		}

		private static void RegisterSettings(IUnityContainer container)
		{
			string dataPath = HostingEnvironment.MapPath("~/App_Data");
			var settings = new Settings(dataPath);
			container.RegisterInstance<ISettings>(settings);
		}
	}
}