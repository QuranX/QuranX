using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Vectorhighlight;
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
			SimpleFragListBuilder.MARGIN = 32;
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
			var analyser = AnalyzerProvider.GetAnalyzer();
			var queryParser = new QueryParser(
				Lucene.Net.Util.Version.LUCENE_30,
				Consts.FullTextFieldName,
				analyser);
			queryParser.AllowLeadingWildcard = true;
			queryParser.DefaultOperator = QueryParser.Operator.AND;

			var query = queryParser.Parse(queryString);

			var resultsCollector = TopScoreDocCollector.Create(
				numHits: 9999,
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
			foreach (var scoreDoc in resultsCollector.TopDocs().ScoreDocs.Take(maxResults))
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

			switch (context.ToLowerInvariant())
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