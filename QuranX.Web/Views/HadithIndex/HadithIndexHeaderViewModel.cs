using System.Collections.Generic;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.HadithIndex
{
	public class HadithIndexHeaderViewModel
	{
		public readonly string UrlSoFar;
		public readonly HadithCollection Collection;
		public readonly IEnumerable<string> IndexPartsAndValues;

		public HadithIndexHeaderViewModel(
			string urlSoFar,
			HadithCollection collection,
			IEnumerable<string> indexPartsAndValues)
		{
			UrlSoFar = urlSoFar;
			Collection = collection;
			IndexPartsAndValues = indexPartsAndValues;
		}
	}
}