using Lucene.Net.Analysis;
using Lucene.Net.Analysis.TokenAttributes;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.VectorHighlight;
using QuranX.Persistence.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Services;

public interface ISearchEngine
{
    IEnumerable<SearchResult> Search(
        string queryString,
        string context,
        string subContext,
        out int totalResults,
        int maxResults = 100);
}

public class SearchEngine : ISearchEngine
{
    private readonly ILuceneAnalyzerProvider AnalyzerProvider;
    private readonly ILuceneIndexSearcherProvider SearcherProvider;

    public SearchEngine(
        ILuceneAnalyzerProvider analyzerProvider,
        ILuceneIndexSearcherProvider searcherProvider)
    {
        AnalyzerProvider = analyzerProvider;
        SearcherProvider = searcherProvider;
    }

    public IEnumerable<SearchResult> Search(
        string queryString,
        string context,
        string subContext,
        out int totalResults,
        int maxResults)
    {
        totalResults = 0;
        queryString = (queryString ?? "").Replace(":", " ");
        if (string.IsNullOrWhiteSpace(queryString))
            return new List<SearchResult>();

        // Cap the length so a pathologically long query can't drive parsing/search cost.
        if (queryString.Length > Consts.MaxQueryLength)
            queryString = queryString.Substring(0, Consts.MaxQueryLength);

        IndexSearcher indexSearcher = SearcherProvider.GetIndexSearcher();
        Analyzer analyzer = AnalyzerProvider.GetAnalyzer();

        var queryParser = new QueryParser(Consts.LuceneVersion, Consts.FullTextFieldName, analyzer)
        {
            // Leading wildcards (e.g. "*foo" or a bare "*") force Lucene to enumerate the
            // entire term dictionary — a cheap DoS on a public endpoint. Keep them disabled.
            AllowLeadingWildcard = false,
            DefaultOperator = Operator.OR
        };

        Query userQuery = ParseUserQuery(queryParser, analyzer, queryString);
        BooleanQuery mainQuery = AddContextCriteria(userQuery, context, subContext);

        // Perform the search and get the top documents
        TopDocs topDocs = indexSearcher.Search(mainQuery, 9999);
        totalResults = topDocs.TotalHits;

        var result = new List<SearchResult>();

        // Initialize the highlighter
        var highlighter = new FastVectorHighlighter();
        FieldQuery fieldQuery = highlighter.GetFieldQuery(mainQuery, indexSearcher.IndexReader); // Use IndexReader

        foreach (var scoreDoc in topDocs.ScoreDocs.Take(maxResults))
        {
            string[] fragments = highlighter.GetBestFragments(
                fieldQuery: fieldQuery,
                reader: indexSearcher.IndexReader,
                docId: scoreDoc.Doc,
                fieldName: Consts.FullTextFieldName,
                fragCharSize: 150, // Adjust fragment size as needed
                maxNumFragments: 5);

            var doc = indexSearcher.Doc(scoreDoc.Doc);
            var searchResult = new SearchResult(
                type: doc.Get(Consts.SerializedObjectTypeFieldName),
                document: doc,
                snippets: fragments
            );
            result.Add(searchResult);
        }

        return result;
    }

    // Parses raw user input into a Lucene query. Malformed query syntax — unbalanced
    // quotes, a trailing operator, a reserved keyword like AND/OR, a stray special
    // character — throws ParseException, which would otherwise surface as a 500. Fall
    // back to a query built directly from the analyzer's tokens (an OR of the terms),
    // which bypasses query syntax entirely and can never throw on the user's input.
    private static Query ParseUserQuery(QueryParser queryParser, Analyzer analyzer, string queryString)
    {
        try
        {
            return queryParser.Parse(queryString);
        }
        catch (ParseException)
        {
            return BuildTermFallbackQuery(analyzer, queryString);
        }
    }

    // Tokenizes the raw input with the same analyzer used at index time and ORs the
    // resulting terms together. An input that yields no tokens (e.g. a bare "*")
    // produces an empty query that simply matches nothing — no exception either way.
    private static Query BuildTermFallbackQuery(Analyzer analyzer, string queryString)
    {
        var fallbackQuery = new BooleanQuery();
        using TokenStream tokenStream = analyzer.GetTokenStream(Consts.FullTextFieldName, queryString);
        ICharTermAttribute termAttribute = tokenStream.AddAttribute<ICharTermAttribute>();
        tokenStream.Reset();
        while (tokenStream.IncrementToken())
        {
            string term = termAttribute.ToString();
            if (term.Length == 0)
                continue;
            fallbackQuery.Add(new TermQuery(new Term(Consts.FullTextFieldName, term)), Occur.SHOULD);
        }
        tokenStream.End();
        return fallbackQuery;
    }

    private BooleanQuery AddContextCriteria(Query mainQuery, string context, string subContext)
    {
        BooleanQuery booleanQuery = new BooleanQuery
            {
                { mainQuery, Occur.MUST }
            };

        if (string.IsNullOrWhiteSpace(context))
            return booleanQuery;

        switch (context.ToUpperInvariant())
        {
            case SearchContexts.Quran:
                var quranCriteria = new TermQuery(new Term(Consts.SerializedObjectTypeFieldName, nameof(Verse)));
                booleanQuery.Add(quranCriteria, Occur.MUST);
                break;

            case SearchContexts.Commentaries:
                var commentariesCriteria = new TermQuery(new Term(Consts.SerializedObjectTypeFieldName, nameof(Commentary)));
                booleanQuery.Add(commentariesCriteria, Occur.MUST);

                if (!string.IsNullOrWhiteSpace(subContext))
                {
                    var subContextCriteria = new TermQuery(new Term($"{nameof(Commentary)}_{nameof(Commentary.CommentatorCode)}", subContext));
                    booleanQuery.Add(subContextCriteria, Occur.MUST);
                }
                break;

            case SearchContexts.Hadiths:
                var hadithsCriteria = new TermQuery(new Term(Consts.SerializedObjectTypeFieldName, nameof(Hadith)));
                booleanQuery.Add(hadithsCriteria, Occur.MUST);

                if (!string.IsNullOrWhiteSpace(subContext))
                {
                    var subContextCriteria = new TermQuery(new Term($"{nameof(Hadith)}_{nameof(Hadith.CollectionCode)}", subContext));
                    booleanQuery.Add(subContextCriteria, Occur.MUST);
                }
                break;

            default:
                return booleanQuery;
        }

        return booleanQuery;
    }

}
