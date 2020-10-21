using System;
using System.Collections.Generic;

namespace QuranX.Web.Views.RootAnalysis
{
	public class WordTypeViewModel
	{
		public string Type { get; }
		public IEnumerable<WordFormViewModel> WordForms { get; }

		public WordTypeViewModel(string type, IEnumerable<WordFormViewModel> wordForms)
		{
			Type = type;
			WordForms = wordForms ?? throw new ArgumentNullException(nameof(wordForms));
		}
	}
}