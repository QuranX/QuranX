using Lucene.Net.Documents;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Models;

public class SearchResult
{
    public readonly string Type;
    public readonly Document Document;
    public readonly string[] Snippets;

    public SearchResult(
        string type,
        Document document,
        IEnumerable<string> snippets)
    {
        Type = type;
        Document = document;
        if (string.Compare(type, "Verse", true) == 0)
            snippets = snippets.Take(1);
        Snippets = snippets.ToArray();
    }
}
