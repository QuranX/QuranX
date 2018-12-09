using System;
using System.Collections.Generic;

namespace QuranX.Web.Views.RootAnalysis
{
	public class ViewModel
	{
		public string ArabicRoot { get; }
		public string RootLetterNames { get; }
		public IEnumerable<WordTypeViewModel> Types { get; }

		public ViewModel(
			string arabicRoot,
			string rootLetterNames,
			IEnumerable<WordTypeViewModel> types)
		{
			ArabicRoot = arabicRoot;
			RootLetterNames = rootLetterNames;
			Types = types ?? throw new ArgumentNullException(nameof(types));
		}
	}
}