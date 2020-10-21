using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class WordsDocument
	{
		readonly Dictionary<string, Word> ReferencesByRoot;

		public WordsDocument()
		{
			this.ReferencesByRoot = new Dictionary<string, Word>(StringComparer.InvariantCultureIgnoreCase);
		}

		public void AddRootWordReferenceGroup(
			string root, 
			IEnumerable<WordReference> references)
		{
			var newRootReferences = new Word(
					text: root,
					references: references
				);
			ReferencesByRoot.Add(root, newRootReferences);
		}

		public IEnumerable<Word> WordReferences
		{
			get
			{
				return ReferencesByRoot.Values
					.OrderBy(x => x.Text);
			}
		}

		public Word this[string root]
		{
			get
			{
				Word result;
				ReferencesByRoot.TryGetValue(root, out result);
				return result;
			}
		}

	}
}
