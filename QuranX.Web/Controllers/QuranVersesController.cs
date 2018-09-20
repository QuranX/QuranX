using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;

namespace QuranX.Web.Controllers
{
	public class QuranVersesController : Controller
	{
		private readonly IVerseRepository _verseRepository;

		public QuranVersesController(IVerseRepository verseRepository)
		{
			_verseRepository = verseRepository;
		}

		public async Task<ActionResult> Index(string verses)
		{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			IEnumerable<VerseRangeReference> verseRangeReferences = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			Verse[] retrievedVerses = (await _verseRepository.GetVerses(verseRangeReferences))
				.OrderBy(x => x.ChapterNumber)
				.ThenBy(x => x.VerseNumber)
				.ToArray();

			var viewModel = new List<Verse>();
			foreach(VerseRangeReference verseRangeReference in verseRangeReferences)
			{
				viewModel.AddRange(retrievedVerses.Where(x => verseRangeReference.Includes(x.ChapterNumber, x.VerseNumber)));
			}
			sw.Stop();
			System.Diagnostics.Trace.WriteLine("===========" + sw.ElapsedMilliseconds);
			return View(viewModel);
		}
	}
}