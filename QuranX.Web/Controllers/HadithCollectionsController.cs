using System.Collections.Generic;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class HadithCollectionsController : Controller
	{
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public HadithCollectionsController(IHadithCollectionRepository hadithCollectionRepository)
		{
			HadithCollectionRepository = hadithCollectionRepository;
		}

		public ActionResult Index()
		{
			IEnumerable<HadithCollection> viewModel = HadithCollectionRepository.GetAll();
			return View("HadithCollections", viewModel);
		}
	}
}