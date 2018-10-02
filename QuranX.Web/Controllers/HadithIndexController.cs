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
		public ActionResult Index(string collectionCode, string indexCode,
			string indexValue1, string indexValue2, string indexValue3)
		{
			var indexNamesAndValues = new List<(string indexPartName, int index, string suffix)>();
			if (!string.IsNullOrWhiteSpace(indexValue1))
				indexNamesAndValues.Add(HadithReference.SplitNameAndValue(indexValue1));
			if (!string.IsNullOrWhiteSpace(indexValue2))
				indexNamesAndValues.Add(HadithReference.SplitNameAndValue(indexValue2));
			if (!string.IsNullOrWhiteSpace(indexValue3))
				indexNamesAndValues.Add(HadithReference.SplitNameAndValue(indexValue3));
			IEnumerable<(int index, string suffix)> indexValues =
				indexNamesAndValues.Select(x => (x.index, x.suffix));
			List<HadithReference> hadithReferences =
				HadithRepository.GetReferences(
					collectionCode: collectionCode,
					indexCode: indexCode,
					values: indexValues)
				.ToList();
			return View();
		}
	}
}