using System.Collections.Generic;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithIndexHeaderViewModel
	{
		public readonly HadithCollection Collection;
		public readonly IEnumerable<string> IndexPartsAndValues;

		public HadithIndexHeaderViewModel(HadithCollection collection, IEnumerable<string> indexPartsAndValues)
		{
			Collection = collection;
			IndexPartsAndValues = indexPartsAndValues;
		}
	}
}