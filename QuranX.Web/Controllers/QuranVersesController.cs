using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds)]
	public class QuranVersesController : Controller
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly IVerseRepository VerseRepository;

		public QuranVersesController(IChapterRepository chapterRepository, IVerseRepository verseRepository)
		{
			ChapterRepository = chapterRepository;
			VerseRepository = verseRepository;
		}

		public ActionResult Index(string verses)
		{
			IEnumerable<VerseRangeReference> verseRangeReferences = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			Verse[] retrievedVerses = VerseRepository.GetVerses(verseRangeReferences)
				.OrderBy(x => x.ChapterNumber)
				.ThenBy(x => x.VerseNumber)
				.ToArray();

			var viewModel = new List<ChapterAndVerseSelection>();
			foreach (VerseRangeReference verseRangeReference in verseRangeReferences)
			{
				Verse[] currentSelection = retrievedVerses.Where(x => verseRangeReference.Includes(x.ChapterNumber, x.VerseNumber)).ToArray();
				var chapterAndSelection = new ChapterAndVerseSelection(ChapterRepository.Get(verseRangeReference.Chapter), currentSelection);
				viewModel.Add(chapterAndSelection);
			}
			return View(viewModel);
		}
	}
}