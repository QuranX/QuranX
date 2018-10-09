using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithViewModel
	{
		public readonly Hadith Hadith;
		public readonly IReadOnlyCollection<KeyValuePair<string, string>> References;

		public HadithViewModel(
			Hadith hadith,
			IEnumerable<KeyValuePair<string, string>> references)
		{
			Hadith = hadith;
			References = references.ToList().AsReadOnly();
		}
	}
}