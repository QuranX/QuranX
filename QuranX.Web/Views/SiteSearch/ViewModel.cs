using System.Collections.Generic;
using System.Web.Mvc;

namespace QuranX.Web.Views.Search
{
	public class ViewModel
	{
		public string Q { get; set; }
		public IEnumerable<SelectListItem> Context { get; }
		public IEnumerable<SearchResultWithLink> SearchResults { get; }
		public int TotalResults { get; }

		public ViewModel(
			string q,
			IEnumerable<SelectListItem> context,
			IEnumerable<SearchResultWithLink> searchResults,
			int totalResults)
		{
			Q = q;
			Context = context;
			SearchResults = searchResults;
			TotalResults = totalResults;
		}
	}
}