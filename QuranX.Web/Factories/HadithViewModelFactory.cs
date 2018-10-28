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
				var references = new List<KeyValuePair<string, string>>();
				foreach (HadithReference reference in hadith.References)
				{
					HadithReferenceDefinition referenceDefinition =
						collection.GetReferenceDefinition(reference.ReferenceCode);
					string referenceName = referenceDefinition.Name;
					string path = reference.ToString(referenceDefinition);
					references.Add(new KeyValuePair<string, string>(referenceName, path));
				}
				var viewModel = new HadithViewModel(
					collectionName: collection.Name,
					hadith: hadith,
					references: references.OrderBy(x => x.Key));
				result.Add(viewModel);
			}
			return result;
		}
	}
}