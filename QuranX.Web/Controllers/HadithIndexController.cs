using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.Controllers
{
	public class HadithIndexController : Controller
	{
		private readonly IHadithRepository HadithRepository;

		public HadithIndexController(IHadithRepository hadithRepository)
		{
			HadithRepository = hadithRepository;
		}

		// GET: HadithReferences
		public ActionResult Index(string collectionCode, string indexCode)
		{
			IEnumerable<HadithReference> hadithReferences =
				HadithRepository.GetReferences(
					collectionCode: collectionCode,
					indexCode: indexCode,
					values: null);
			IEnumerable<string> values =
				hadithReferences
				.OrderBy(x => x.IndexPart1)
				.ThenBy(x => x.IndexPart1Suffix.Length)
				.ThenBy(x => x.IndexPart1Suffix)
				.Select(x => x.IndexPart1 + x.IndexPart1Suffix)
				.Distinct()
				.ToList();
			return View();
		}
	}
}