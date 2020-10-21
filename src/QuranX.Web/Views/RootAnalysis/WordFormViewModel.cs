using System;
using System.Collections.Generic;

namespace QuranX.Web.Views.RootAnalysis
{
	public class WordFormViewModel
	{
		public string Form { get; }
		public IEnumerable<VerseViewModel> Extracts { get; }

		public WordFormViewModel(string form, IEnumerable<VerseViewModel> extracts)
		{
			Form = form;
			Extracts = extracts ?? throw new ArgumentNullException(nameof(extracts));
		}
	}
}