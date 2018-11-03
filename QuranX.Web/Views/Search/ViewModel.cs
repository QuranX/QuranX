using System.Collections.Generic;
using System.Web.Mvc;

namespace QuranX.Web.Views.Search
{
	public class ViewModel
	{
		public string Q { get; set; }
		public IEnumerable<SelectListItem> Context { get; set; }

		public ViewModel(string q, IEnumerable<SelectListItem> context)
		{
			Q = q;
			Context = context;
		}
	}
}