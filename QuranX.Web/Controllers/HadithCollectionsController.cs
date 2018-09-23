using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.Controllers
{
	public class HadithCollectionsController : Controller
	{
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public HadithCollectionsController(IHadithCollectionRepository hadithCollectionRepository)
		{
			HadithCollectionRepository = hadithCollectionRepository;
		}

		public ActionResult Index()
		{
			HadithCollection[] viewModel = HadithCollectionRepository.GetAll();
			return View(viewModel);
		}
	}
}