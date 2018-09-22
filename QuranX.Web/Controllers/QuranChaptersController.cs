using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

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
			ChapterAndVerseReferenceSelection[] viewModel = ChapterRepository.GetAll()
				.Select(x => new ChapterAndVerseReferenceSelection(
					chapter: x,
					verseReferences: new VerseRangeReference(
						chapter: x.ChapterNumber,
						firstVerse: 1,
						lastVerse: x.NumberOfVerses).ToVerseReferences()))
				.ToArray();
			return View(viewModel);
		}
	}
}