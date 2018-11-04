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
			Persistence.Services.Registration.Register(container);
			QuranX.Web.Services.Registration.Register(container);
			DependencyResolver.SetResolver(new UnityDependencyResolver(container));
		}

	}
}