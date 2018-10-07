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
			IEnumerable<HadithReference> hadithReferences =
				HadithRepository.GetReferences(
					collectionCode: collectionCode,
					indexCode: indexCode,
					values: indexValues);

			IEnumerable<string> urlIndexParts = indexNamesAndValues
				.Select(x => x.index + x.suffix)
				.Select((value, index) => indexDefinition.PartNames[index] + "-" + value);
			string partsAsUrl = string.Join("/", urlIndexParts);
			var headerViewModel = new HadithIndexHeaderViewModel(
				$"/hadith/{collectionCode}/{indexCode}/{partsAsUrl}",
				collection,
				urlIndexParts);
			if (indexPartNames.Count() == indexDefinition.PartNames.Count)
			{
				IEnumerable<int> hadithIds = hadithReferences.Select(x => x.HadithId);
				IEnumerable<Hadith> hadiths = HadithRepository.GetHadiths(hadithIds);
				var viewModel = new HadithsViewModel(headerViewModel, hadiths);
				return View("Hadiths", viewModel);
			}
			else
			{
				string nextIndexPartName = indexDefinition.PartNames[indexPartNames.Count()];
				Func<HadithReference, string> getNextValue;
				switch (indexPartNames.Count())
				{
					case 0:
						getNextValue = x => x.IndexPart1 + x.IndexPart1Suffix;
						break;
					case 1:
						getNextValue = x => x.IndexPart2 + x.IndexPart2Suffix;
						break;
					case 2:
						getNextValue = x => x.IndexPart3 + x.IndexPart3Suffix;
						break;
					default:
						throw new NotImplementedException();
				}
				// If the next level is the final level (the hadith itself) then remove the suffix
				// from the final part so that all hadiths with the same index but different
				// suffixes are shown on screen at once.
				hadithReferences = hadithReferences
					.Select(x => x.ExcludingFinalSuffix())
					.Distinct();
				// Get the next available values
				IEnumerable<string> nextIndexPartValues =
					hadithReferences.Select(getNextValue)
					.Distinct();
				var viewModel = new BrowseHadithIndexViewModel(
					hadithIndexHeaderViewModel: headerViewModel,
					nextIndexPartName: nextIndexPartName,
					nextIndexPartValues: nextIndexPartValues);
				return View("BrowseHadithIndex", viewModel);
			}
		}

	}
}