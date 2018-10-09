using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithsViewModel
	{
		public readonly IReadOnlyCollection<HadithViewModel> Hadiths;
		public readonly HadithIndexHeaderViewModel HadithIndexHeaderViewModel;

		public HadithsViewModel(
			HadithIndexHeaderViewModel hadithIndexHeaderViewModel,
			IEnumerable<HadithViewModel> hadiths)
		{
			HadithIndexHeaderViewModel = hadithIndexHeaderViewModel;
			Hadiths = hadiths.ToList().AsReadOnly();
		}
	}
}