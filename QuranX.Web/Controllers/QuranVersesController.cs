using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Controllers
{
	public class QuranVersesController : Controller
	{
		private readonly IChapterRepository _chapterRepository;
		private readonly IVerseRepository _verseRepository;

		public QuranVersesController(IChapterRepository chapterRepository, IVerseRepository verseRepository)
		{
			_chapterRepository = chapterRepository;
			_verseRepository = verseRepository;
		}

		public async Task<ActionResult> Index(string verses)
		{
			IEnumerable<VerseRangeReference> verseRangeReferences = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			Verse[] retrievedVerses = (await _verseRepository.GetVerses(verseRangeReferences))
				.OrderBy(x => x.ChapterNumber)
				.ThenBy(x => x.VerseNumber)
				.ToArray();

			var viewModel = new List<ChapterAndVerseSelection>();
			foreach(VerseRangeReference verseRangeReference in verseRangeReferences)
			{
				Verse[] currentSelection = retrievedVerses.Where(x => verseRangeReference.Includes(x.ChapterNumber, x.VerseNumber)).ToArray();
				var chapterAndSelection = new ChapterAndVerseSelection(_chapterRepository.Get(verseRangeReference.Chapter), currentSelection);
				viewModel.Add(chapterAndSelection);
			}
			return View(viewModel);
		}
	}
}