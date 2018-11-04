using System.Collections.Generic;
using System.Web.Mvc;

namespace QuranX.Web.Views.Search
{
	public class ViewModel
	{
		public string Q { get; set; }
		public IEnumerable<SelectListItem> Context { get; set; }
		public IEnumerable<SearchResultWithLink> SearchResults { get; }

		public ViewModel(string q, IEnumerable<SelectListItem> context, IEnumerable<SearchResultWithLink> searchResults)
		{
			Q = q;
			Context = context;
			SearchResults = searchResults;
		}
	}
}