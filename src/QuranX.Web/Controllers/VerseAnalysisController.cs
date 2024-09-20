using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using QuranX.Web.Views.VerseAnalysis;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class VerseAnalysisController : Controller
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly IVerseAnalysisRepository VerseAnalysisRepository;

		public VerseAnalysisController(
			IChapterRepository chapterRepository,
			IVerseAnalysisRepository verseAnalysisRepository)
		{
			ChapterRepository = chapterRepository;
			VerseAnalysisRepository = verseAnalysisRepository;
		}

		public ActionResult Index(int chapterNumber, int verseNumber)
		{
			if (!QuranStructure.TryValidateChapterAndVerse(chapterNumber, verseNumber))
				return NotFound();

			Chapter chapter = ChapterRepository.Get(chapterNumber);
			VerseAnalysis analysis =
				VerseAnalysisRepository.GetForVerse(chapterNumber, verseNumber);

			var selectChapterAndVerse = new SelectChapterAndVerse(
				selectedChapterNumber: chapterNumber,
				selectedVerseNumber: verseNumber,
				url: "/Analysis/");
			var viewModel = new ViewModel(
				chapter: chapter,
				verseNumber: verseNumber,
				verseAnalysis: analysis,
				selectChapterAndVerse: selectChapterAndVerse);
			return View("VerseAnalysis", viewModel);
		}
	}
}