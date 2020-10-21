using System.Collections.Generic;
using System.Linq;
using QuranX.Web.Views.Shared;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithsViewModel
	{
		public readonly IReadOnlyList<HadithViewModel> Hadiths;
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