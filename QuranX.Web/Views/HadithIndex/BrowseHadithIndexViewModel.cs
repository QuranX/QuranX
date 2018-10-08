using System.Collections.Generic;

namespace QuranX.Web.Views.HadithIndex
{
	public class BrowseHadithIndexViewModel
	{
		public readonly HadithIndexHeaderViewModel HadithIndexHeaderViewModel;
		public readonly string NextReferencePartName;
		public readonly IEnumerable<string> NextReferencePartValueSelection;

		public BrowseHadithIndexViewModel(
			HadithIndexHeaderViewModel hadithIndexHeaderViewModel,
			string nextReferencePartName,
			IEnumerable<string> nextReferencePartValueSelection)
		{
			HadithIndexHeaderViewModel = hadithIndexHeaderViewModel;
			NextReferencePartName = nextReferencePartName;
			NextReferencePartValueSelection = nextReferencePartValueSelection;
		}
	}
}