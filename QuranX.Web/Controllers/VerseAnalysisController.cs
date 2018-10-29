using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Views.VerseAnalysis;

namespace QuranX.Web.Controllers
{
	public class VerseAnalysisController : Controller
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly IVerseAnalysisWordRepository VerseAnalysisWordRepository;

		public VerseAnalysisController(
			IChapterRepository chapterRepository,
			IVerseAnalysisWordRepository verseAnalysisWordRepository)
		{
			ChapterRepository = chapterRepository;
			VerseAnalysisWordRepository = verseAnalysisWordRepository;
		}

		public ActionResult Index(int chapterNumber, int verseNumber)
		{
			if (!QuranStructure.TryValidateChapterAndVerse(chapterNumber, verseNumber))
				return HttpNotFound();

			Chapter chapter = ChapterRepository.Get(chapterNumber);
			IEnumerable<VerseAnalysisWord> analysis =
				VerseAnalysisWordRepository.GetForVerse(chapterNumber, verseNumber)
				.OrderBy(x => x.WordNumber);

			var viewModel = new ViewModel(
				chapter: chapter,
				verseNumber: verseNumber,
				words: analysis);
			return View(viewModel);
		}
	}
}