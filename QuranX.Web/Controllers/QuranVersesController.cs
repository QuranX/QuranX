using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using QuranX.Web.Views.QuranVerses;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
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
			if (!verseRangeReferences.Any())
				verseRangeReferences = new VerseRangeReference[] { new VerseRangeReference(1, 1, 1) };

			IEnumerable<Verse> retrievedVerses = VerseRepository.GetVerses(verseRangeReferences)
				.OrderBy(x => x.ChapterNumber)
				.ThenBy(x => x.VerseNumber);

			var displayVerses = new List<ChapterAndVerseSelection>();
			foreach (VerseRangeReference verseRangeReference in verseRangeReferences)
			{
				IEnumerable<Verse> currentSelection =
					retrievedVerses
					.Where(x => verseRangeReference.Includes(x.ChapterNumber, x.VerseNumber));
				var chapterAndSelection = new ChapterAndVerseSelection(ChapterRepository.Get(verseRangeReference.Chapter), currentSelection);
				displayVerses.Add(chapterAndSelection);
			}

			IEnumerable<ChapterAndVerseReferenceSelection> allVerses = VerseRepository.GetVerseReferences()
				.GroupBy(x => x.Chapter)
				.Select(x => new ChapterAndVerseReferenceSelection(
					chapter: ChapterRepository.Get(x.Key),
					verseReferences: x));

			VerseRangeReference firstReference = verseRangeReferences.First();
			var viewModel = new ViewModel(
				displayVerses: displayVerses,
				selectChapterAndVerse: new SelectChapterAndVerse(
					firstReference.Chapter,
					firstReference.FirstVerse,
					allVerses)
				); 
			return View(viewModel);
		}
	}
}