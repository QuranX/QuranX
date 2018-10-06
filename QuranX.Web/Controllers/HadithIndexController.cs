using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Views.HadithIndex;

namespace QuranX.Web.Controllers
{
	public class HadithIndexController : Controller
	{
		private readonly IHadithRepository HadithRepository;
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public HadithIndexController(
			IHadithRepository hadithRepository,
			IHadithCollectionRepository hadithCollectionRepository)
		{
			HadithRepository = hadithRepository;
			HadithCollectionRepository = hadithCollectionRepository;
		}

		// GET: HadithReferences
		public ActionResult Index(string collectionCode, string indexCode,
			string indexValue1, string indexValue2, string indexValue3)
		{
			HadithCollection collection = HadithCollectionRepository.Get(collectionCode);
			if (collection == null)
				return HttpNotFound();

			HadithReferenceDefinition indexDefinition = collection.GetReferenceDefinition(indexCode);
			if (indexDefinition == null)
				return HttpNotFound();

			var indexNamesAndValues = new List<(string indexPartName, int index, string suffix)>();
			if (!string.IsNullOrWhiteSpace(indexValue1))
				indexNamesAndValues.Add(HadithReference.SplitNameAndValue(indexValue1));
			if (!string.IsNullOrWhiteSpace(indexValue2))
				indexNamesAndValues.Add(HadithReference.SplitNameAndValue(indexValue2));
			if (!string.IsNullOrWhiteSpace(indexValue3))
				indexNamesAndValues.Add(HadithReference.SplitNameAndValue(indexValue3));

			IEnumerable<string> indexPartNames = indexNamesAndValues.Select(x => x.indexPartName);
			if (!indexDefinition.PatternMatch(indexPartNames))
				return HttpNotFound();

			IEnumerable<(int index, string suffix)> indexValues =
				indexNamesAndValues.Select(x => (x.index, x.suffix));
			List<HadithReference> hadithReferences =
				HadithRepository.GetReferences(
					collectionCode: collectionCode,
					indexCode: indexCode,
					values: indexValues)
				.ToList();

			IEnumerable<string> urlIndexParts = indexNamesAndValues
				.Select(x => x.index + x.suffix)
				.Select((value, index) => indexDefinition.PartNames[index] + "-" + value);
			var headerViewModel = new HadithIndexHeaderViewModel(collection, urlIndexParts);
			if (indexPartNames.Count() == indexDefinition.PartNames.Count())
			{
				IEnumerable<int> hadithIds = hadithReferences.Select(x => x.HadithId);
				IEnumerable<Hadith> hadiths = HadithRepository.GetHadiths(hadithIds);
				var viewModel = new HadithsViewModel(headerViewModel, hadiths);
				return View("Hadiths", viewModel);
			}
			else
			{
				return HttpNotFound();
			}
		}

	}
}