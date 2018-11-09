using System;
using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Views.Shared;

namespace QuranX.Web.Factories
{
	public interface IHadithViewModelFactory
	{
		IEnumerable<HadithViewModel> Create(IEnumerable<Hadith> hadiths);
	}

	public class HadithViewModelFactory : IHadithViewModelFactory
	{
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public HadithViewModelFactory(IHadithCollectionRepository hadithCollectionRepository)
		{
			HadithCollectionRepository = hadithCollectionRepository;
		}

		public IEnumerable<HadithViewModel> Create(IEnumerable<Hadith> hadiths)
		{
			var result = new List<HadithViewModel>();
			foreach (Hadith hadith in hadiths)
			{
				string collectionCode = hadith.References[0].CollectionCode;
				HadithCollection collection = HadithCollectionRepository.Get(collectionCode);
				var references = new List<HadithReferenceViewModel>();
				foreach (HadithReference reference in hadith.References)
				{
					HadithReferenceDefinition referenceDefinition =
						collection.GetReferenceDefinition(reference.ReferenceCode);
					var referenceViewModel = new HadithReferenceViewModel(
						collectionCode: collection.Code,
						collectionName: collection.Name,
						indexCode: referenceDefinition.Code,
						indexName: referenceDefinition.Name,
						partNamesAndValues: reference.ToNameValuePairs(referenceDefinition));
					references.Add(referenceViewModel);
				}
				var viewModel = new HadithViewModel(
					collectionName: collection.Name,
					hadith: hadith,
					references: references);
				result.Add(viewModel);
			}
			return result;
		}
	}
}