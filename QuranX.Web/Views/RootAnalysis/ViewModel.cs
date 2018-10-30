using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Views.RootAnalysis
{
	public class ViewModel
	{
		public string ArabicRoot { get; }
		public string RootLetterNames { get; }
		public IReadOnlyList<VerseViewModel> Extracts { get; }

		public ViewModel(
			string arabicRoot,
			string rootLetterNames,
			IReadOnlyList<VerseViewModel> extracts)
		{
			if (extracts == null)
				throw new ArgumentNullException(nameof(extracts));

			ArabicRoot = arabicRoot;
			RootLetterNames = rootLetterNames;
			Extracts = extracts.ToList().AsReadOnly();
		}
	}
}