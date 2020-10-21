using System;
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

		public ActionResult Index(string verses, int? context)
		{
			IEnumerable<VerseRangeReference> verseRangeReferences = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			if (!verseRangeReferences.Any())
				verseRangeReferences = new VerseRangeReference[] { new VerseRangeReference(1, 1, 1) };

			VerseRangeReference firstReference = verseRangeReferences.First();
			bool autoScrollToSelectedVerse = verseRangeReferences.Count() == 1 && context.HasValue && context > 0;
			if (autoScrollToSelectedVerse)
				verseRangeReferences = AddSurroundingVerses(context, firstReference);

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

			var viewModel = new ViewModel(
				displayVerses: displayVerses,
				selectChapterAndVerse: new SelectChapterAndVerse(
					firstReference.Chapter,
					firstReference.FirstVerse,
					url: ""),
				autoScrollToSelectedVerse: autoScrollToSelectedVerse
				); 
			return View("QuranVerses", viewModel);
		}

		private static IEnumerable<VerseRangeReference> AddSurroundingVerses(int? context, VerseRangeReference firstReference)
		{
			IEnumerable<VerseRangeReference> verseRangeReferences;
			int numberOfVerses = QuranStructure.Chapter(firstReference.Chapter).NumberOfVerses;
			int firstVerse = Math.Max(1, firstReference.FirstVerse - context.Value);
			int lastVerse = Math.Min(numberOfVerses, firstReference.LastVerse + context.Value);
			var newVerseReference = new VerseRangeReference(
				chapter: firstReference.Chapter,
				firstVerse: firstVerse,
				lastVerse: lastVerse);
			verseRangeReferences = new[] { newVerseReference };
			return verseRangeReferences;
		}
	}
}