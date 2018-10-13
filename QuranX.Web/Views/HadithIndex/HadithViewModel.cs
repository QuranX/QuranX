using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithViewModel
	{
		public readonly string CollectionName;
		public readonly Hadith Hadith;
		public readonly IReadOnlyCollection<KeyValuePair<string, string>> References;

		public HadithViewModel(
			string collectionName,
			Hadith hadith,
			IEnumerable<KeyValuePair<string, string>> references)
		{
			CollectionName = collectionName;
			Hadith = hadith;
			References = references.ToList().AsReadOnly();
		}
	}
}