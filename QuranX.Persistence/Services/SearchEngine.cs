using System.Collections.Generic;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Vectorhighlight;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services
{
	public interface ISearchEngine
	{
		IEnumerable<SearchResult> Search(string queryString, out int totalResults, int maxResults = 100);
	}

	public class SearchEngine : ISearchEngine
	{
		private readonly ILuceneAnalyzerProvider AnalyzerProvider;
		private readonly ILuceneIndexSearcherProvider SearcherProvider;

		public SearchEngine(
			ILuceneAnalyzerProvider analyzerProvider,
			ILuceneIndexSearcherProvider searcherProvider)
		{
			SimpleFragListBuilder.MARGIN = 32;
			AnalyzerProvider = analyzerProvider;
			SearcherProvider = searcherProvider;
		}

		public IEnumerable<SearchResult> Search(
			string queryString,
			out int totalResults,
			int maxResults = 100)
		{
#if DEBUG
			maxResults = 2000;
#endif
			totalResults = 0;
			queryString = (queryString ?? "").Replace(":", " ");
			if (string.IsNullOrWhiteSpace(queryString))
				return new List<SearchResult>();

			IndexSearcher indexSearcher = SearcherProvider.GetIndexSearcher();
			var analyser = AnalyzerProvider.GetAnalyzer();
			var queryParser = new QueryParser(
				Lucene.Net.Util.Version.LUCENE_30,
				Consts.FullTextFieldName,
				analyser);
			queryParser.AllowLeadingWildcard = true;
			queryParser.DefaultOperator = QueryParser.Operator.AND;

			var query = queryParser.Parse(queryString);

			var resultsCollector = TopScoreDocCollector.Create(
				numHits: maxResults,
				docsScoredInOrder: true
			);
			indexSearcher.Search(
				query: query,
				results: resultsCollector
			);
			totalResults = resultsCollector.TotalHits;
			var result = new List<SearchResult>();

			var highlighter = new FastVectorHighlighter();
			var fieldQuery = highlighter.GetFieldQuery(query);
			foreach (var scoreDoc in resultsCollector.TopDocs().ScoreDocs)
			{
				string[] fragments = highlighter.GetBestFragments(
					fieldQuery: fieldQuery,
					reader: indexSearcher.IndexReader,
					docId: scoreDoc.Doc,
					fieldName: Consts.FullTextFieldName,
					fragCharSize: 100,
					maxNumFragments: 5);
				var doc = indexSearcher.Doc(scoreDoc.Doc);
				var searchResult = new SearchResult(
					type: doc.Get(Consts.SerializedObjectTypeFieldName),
					id: doc.Get("ID"),
					snippets: fragments
				);
				result.Add(searchResult);
			}
			return result;
		}
	}
}