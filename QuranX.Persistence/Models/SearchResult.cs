using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models
{
	public class SearchResult
	{
		public readonly string Type;
		public readonly string ID;
		public readonly string[] Snippets;

		public SearchResult(
			string type,
			string id,
			IEnumerable<string> snippets)
		{
			Type = type;
			ID = id;
			if (string.Compare(type, "Verse", true) == 0)
			{
				if (snippets.Count() > 1)
					snippets = snippets.Skip(1).Take(1);
			}
			Snippets = snippets.ToArray();
		}
	}

}
