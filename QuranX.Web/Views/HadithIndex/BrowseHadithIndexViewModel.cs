using System.Collections.Generic;

namespace QuranX.Web.Views.HadithIndex
{
	public class BrowseHadithIndexViewModel
	{
		public readonly HadithIndexHeaderViewModel HadithIndexHeaderViewModel;
		public readonly string NextIndexPartName;
		public readonly IEnumerable<string> NextIndexPartValues;

		public BrowseHadithIndexViewModel(
			HadithIndexHeaderViewModel hadithIndexHeaderViewModel,
			string nextIndexPartName,
			IEnumerable<string> nextIndexPartValues)
		{
			HadithIndexHeaderViewModel = hadithIndexHeaderViewModel;
			NextIndexPartName = nextIndexPartName;
			NextIndexPartValues = nextIndexPartValues;
		}
	}
}