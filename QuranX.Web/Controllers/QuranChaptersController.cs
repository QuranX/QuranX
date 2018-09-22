using System.Collections.Generic;
using System.Web.Mvc;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;

namespace QuranX.Web.Controllers
{
	public class QuranChaptersController : Controller
	{
		private readonly IChapterRepository ChapterRepository;

		public QuranChaptersController(IChapterRepository chapterRepository)
		{
			ChapterRepository = chapterRepository;
		}

		public ActionResult Index()
		{
			IEnumerable<Chapter> viewModel = ChapterRepository.GetAll();
			return View(viewModel);
		}
	}
}