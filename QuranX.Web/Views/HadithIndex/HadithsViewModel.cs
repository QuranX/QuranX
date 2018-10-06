using System.Collections.Generic;
using System.Linq;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithsViewModel
	{
		public readonly IReadOnlyCollection<Hadith> Hadiths;
		public readonly HadithIndexHeaderViewModel HadithIndexHeaderViewModel;

		public HadithsViewModel(
			HadithIndexHeaderViewModel hadithIndexHeaderViewModel,
			IEnumerable<Hadith> hadiths)
		{
			HadithIndexHeaderViewModel = hadithIndexHeaderViewModel;
			Hadiths = hadiths.ToList().AsReadOnly();
		}
	}
}