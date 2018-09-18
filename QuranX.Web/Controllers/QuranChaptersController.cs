using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuranX.Persistence.Services;
using QuranX.Shared.Models;

namespace QuranX.Web.Controllers
{
	public class QuranChaptersController : Controller
	{
		private readonly IChapterRepository _chapterRepository;

		public QuranChaptersController(IChapterRepository chapterRepository)
		{
			_chapterRepository = chapterRepository;
		}

		public ActionResult Index()
		{
			IEnumerable<Chapter> viewModel = _chapterRepository.GetAll();
			return View(viewModel);
		}
	}
}