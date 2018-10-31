using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Factories;
using QuranX.Web.Views.Shared;
using QuranX.Web.Views.VerseHadiths;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class VerseHadithsController : Controller
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly IHadithRepository HadithRepository;
		private readonly IHadithViewModelFactory HadithViewModelFactory;

		public VerseHadithsController(
			IChapterRepository chapterRepository,
			IHadithRepository hadithRepository,
			IHadithViewModelFactory hadithViewModelFactory)
		{
			ChapterRepository = chapterRepository;
			HadithRepository = hadithRepository;
			HadithViewModelFactory = hadithViewModelFactory;
		}

		public ActionResult Index(int chapterNumber, int verseNumber)
		{
			if (!QuranStructure.TryValidateChapterAndVerse(chapterNumber, verseNumber))
				return HttpNotFound();

			Chapter chapter = ChapterRepository.Get(chapterNumber);
			var verseReference = new VerseReference(chapterNumber, verseNumber);
			IEnumerable<Persistence.Models.Hadith> hadiths =
				HadithRepository.GetForVerse(verseReference)
				.OrderBy(x => x.References[0]);

			IEnumerable<HadithViewModel> hadithViewModels = HadithViewModelFactory.Create(hadiths);
			var viewModel = new ViewModel(
				chapter: chapter,
				verseNumber: verseNumber,
				hadiths: hadithViewModels);
			return View("VerseHadiths", viewModel);
		}
	}
}