using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Views.RootAnalysis
{
	public class ViewModel
	{
		public string ArabicRoot { get; }
		public string RootLetterNames { get; }
		public IReadOnlyCollection<Persistence.Models.Dictionary> Dictionaries { get; }
		public IEnumerable<WordTypeViewModel> Types { get; }

		public ViewModel(
			string arabicRoot,
			string rootLetterNames,
			IEnumerable<Persistence.Models.Dictionary> dictionaries,
			IEnumerable<WordTypeViewModel> types)
		{
			ArabicRoot = arabicRoot;
			RootLetterNames = rootLetterNames;
			Dictionaries = dictionaries.ToList().AsReadOnly();
			Types = types ?? throw new ArgumentNullException(nameof(types));
		}
	}
}