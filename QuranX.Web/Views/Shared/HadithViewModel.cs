using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.Shared
{
	public class HadithViewModel
	{
		public readonly string CollectionName;
		public readonly Hadith Hadith;
		public readonly IReadOnlyList<HadithReferenceViewModel> References;

		public HadithViewModel(
			string collectionName,
			Hadith hadith,
			IEnumerable<HadithReferenceViewModel> references)
		{
			CollectionName = collectionName;
			Hadith = hadith;
			References = references.ToList().AsReadOnly();
		}
	}
}