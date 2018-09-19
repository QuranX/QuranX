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
			IEnumerable<VerseRangeReference> verseRangeReferences = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			Verse[] model = await _verseRepository.GetVerses(verseRangeReferences);
			return View(model);
		}
	}
}