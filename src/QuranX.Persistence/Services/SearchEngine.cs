using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
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

			IndexSearcher indexSearcher = SearcherProvider.GetIndexSearcher();
			Analyzer analyzer = AnalyzerProvider.GetAnalyzer();

			var queryParser = new QueryParser(Consts.LuceneVersion, Consts.FullTextFieldName, analyzer) {
				AllowLeadingWildcard = true,
				DefaultOperator = Operator.AND
			};


			Query userQuery = queryString.Contains("*") || queryString.Contains("?")
				? ExpandWildcardQuery(Consts.FullTextFieldName, queryString, indexSearcher.IndexReader)
				: queryParser.Parse(queryString);
			
			BooleanQuery mainQuery = CreateFromContextAndSearchQuery(context, subContext, userQuery);

			// Perform the search and get the top documents
			TopDocs topDocs = indexSearcher.Search(mainQuery, 9999);
			totalResults = topDocs.TotalHits;

			var result = new List<SearchResult>();

			var highlighter = new FastVectorHighlighter();
			FieldQuery fieldQuery = highlighter.GetFieldQuery(mainQuery);

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


		private BooleanQuery CreateFromContextAndSearchQuery(string context, string subContext, Query userQuery)
		{
			BooleanQuery booleanQuery = new BooleanQuery
			{
				{ userQuery, Occur.MUST }
			};

			if (string.IsNullOrWhiteSpace(context))
				return booleanQuery;

			switch (context)
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
					throw new ArgumentException($"Unknown context {context} {subContext}", nameof(context));
			}

			return booleanQuery;
		}

		private bool MatchesWildcard(string pattern, string term)
		{
			// Replace * and ? with their regex equivalents
			var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
								.Replace("\\*", ".*")
								.Replace("\\?", ".") + "$";

			return System.Text.RegularExpressions.Regex.IsMatch(term, regexPattern);
		}

		private BooleanQuery ExpandWildcardQuery(string fieldName, string wildcardTerm, IndexReader reader)
		{
			BooleanQuery expandedQuery = new BooleanQuery();

			// Get the terms from the index that match the wildcard
			Terms terms = MultiFields.GetTerms(reader, fieldName);
			if (terms == null) return expandedQuery; // If there are no terms, return empty query

			TermsEnum termsEnum = terms.GetEnumerator();
			while (termsEnum.MoveNext())
			{
				var term = termsEnum.Term.Utf8ToString();
				if (MatchesWildcard(wildcardTerm, term))
				{
					expandedQuery.Add(new TermQuery(new Term(fieldName, term)), Occur.SHOULD);
				}
			}

			return expandedQuery;
		}

	}
}
