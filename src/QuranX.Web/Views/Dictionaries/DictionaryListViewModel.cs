using System;
using System.Collections.Generic;
using QuranX.Persistence.Models;

namespace QuranX.Web.Views.Dictionaries
{
	public class DictionaryListViewModel
	{
		public string CurrentRoot { get; }
		public string ParentRoot { get; }
		public IEnumerable<string> ChildRoots { get; }
		public IEnumerable<Persistence.Models.Dictionary> Dictionaries { get; }

		public DictionaryListViewModel(string currentRoot, IEnumerable<string> childRoots, IEnumerable<Dictionary> dictionaries)
		{
			CurrentRoot = currentRoot;
			if (!string.IsNullOrEmpty(CurrentRoot) && CurrentRoot.Length > 1)
			{
				ParentRoot = CurrentRoot.Substring(0, CurrentRoot.Length - 1);
			}
			ChildRoots = childRoots ?? throw new ArgumentNullException(nameof(childRoots));
			Dictionaries = dictionaries ?? throw new ArgumentNullException(nameof(dictionaries));
		}
	}
}
