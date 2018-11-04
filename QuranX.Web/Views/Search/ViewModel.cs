using System.Collections.Generic;
using System.Web.Mvc;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.Search
{
	public class ViewModel
	{
		public string Q { get; set; }
		public IEnumerable<SelectListItem> Context { get; set; }
		public IEnumerable<SearchResult> SearchResults { get; }

		public ViewModel(string q, IEnumerable<SelectListItem> context, IEnumerable<SearchResult> searchResults)
		{
			Q = q;
			Context = context;
			SearchResults = searchResults;
		}
	}
}