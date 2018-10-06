using System.Collections.Generic;

namespace QuranX.Web.Views.HadithIndex
{
	public class IndexPartsViewModel
	{
		public readonly HadithIndexHeaderViewModel HadithIndexHeaderViewModel;
		public readonly string NextIndexPartName;
		public readonly IEnumerable<string> NextIndexPartValues;

		public IndexPartsViewModel(
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