using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.VectorHighlight;
using Lucene.Net.Util;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services
{
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

			queryString = AddContextCriteria(queryString, context, subContext);

			IndexSearcher indexSearcher = SearcherProvider.GetIndexSearcher();
			Analyzer analyzer = AnalyzerProvider.GetAnalyzer();

			var queryParser = new QueryParser(Consts.LuceneVersion, Consts.FullTextFieldName, analyzer) {
				AllowLeadingWildcard = true,
				DefaultOperator = Operator.AND
			};

			Query query = queryParser.Parse(queryString);

			// Perform the search and get the top documents
			TopDocs topDocs = indexSearcher.Search(query, 9999);
			totalResults = topDocs.TotalHits;

			var result = new List<SearchResult>();

			// Initialize the highlighter
			var highlighter = new FastVectorHighlighter();
			FieldQuery fieldQuery = highlighter.GetFieldQuery(query);

			foreach (var scoreDoc in topDocs.ScoreDocs.Take(maxResults))
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
					document: doc,
					snippets: fragments
				);
				result.Add(searchResult);
			}

			return result;
		}

		private string AddContextCriteria(string queryString, string context, string subContext)
		{
			if (string.IsNullOrWhiteSpace(context))
				return queryString;

			switch (context.ToUpperInvariant())
			{
				case SearchContexts.Quran:
					string quranCriteria = $"{Consts.SerializedObjectTypeFieldName}:{nameof(Verse)}";
					return $"{quranCriteria} AND {queryString}";

				case SearchContexts.Commentaries:
					string commentariesCriteria = $"{Consts.SerializedObjectTypeFieldName}:{nameof(Commentary)}";
					if (!string.IsNullOrWhiteSpace(subContext))
						queryString += $" AND {nameof(Commentary)}_{nameof(Commentary.CommentatorCode)}:{subContext}";
					return $"{commentariesCriteria} AND {queryString}";

				case SearchContexts.Hadiths:
					string hadithsCriteria = $"{Consts.SerializedObjectTypeFieldName}:{nameof(Hadith)}";
					if (!string.IsNullOrWhiteSpace(subContext))
						queryString += $" AND {nameof(Hadith)}_{nameof(Hadith.CollectionCode)}:{subContext}";
					return $"{hadithsCriteria} AND {queryString}";

				default:
					throw new ArgumentException($"Unknown context {context} {subContext}", nameof(context));
			}
		}
	}
}
