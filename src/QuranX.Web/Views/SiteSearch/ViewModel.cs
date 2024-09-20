using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuranX.Web.Views.Search
{
	public class ViewModel
	{
		public string Q { get; set; }
		public IEnumerable<SelectListItem> Context { get; }
		public IEnumerable<SearchResultWithLink> SearchResults { get; }
		public int TotalResults { get; }
		public bool BadQuery { get; }

		public ViewModel(
			string q,
			IEnumerable<SelectListItem> context,
			IEnumerable<SearchResultWithLink> searchResults,
			int totalResults,
			bool badQuery)
		{
			Q = q;
			Context = context;
			SearchResults = searchResults;
			TotalResults = totalResults;
			BadQuery = badQuery;
		}
	}
}