using System;
using System.Linq.Expressions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using QuranX.Persistence.Services;

namespace QuranX.Persistence.Extensions
{
	public static class BooleanQueryExtensions
	{
		public static BooleanQuery FilterByType<T>(this BooleanQuery instance)
		{
			var term = new Term(Consts.SerializedObjectTypeFieldName, typeof(T).Name);
			var query = new PhraseQuery();
			query.Add(term);
			instance.Add(query, Occur.MUST);
			return instance;
		}

		public static BooleanQuery AddPhraseQuery<TObj>(
			this BooleanQuery instance,
			Expression<Func<TObj, string>> expression,
			string value,
			Occur occur)
		{
			string indexName = ExpressionExtensions.GetIndexName(expression);
			return instance.AddPhraseQuery(indexName, value, occur);
		}

		public static BooleanQuery AddPhraseQuery(
			this BooleanQuery instance,
			string indexName,
			string value,
			Occur occur)
		{
			if (value != null)
			{
				value = value.ToLowerInvariant();

				var term = new Term(indexName, value);
				var subQuery = new TermQuery(term);
				instance.Add(subQuery, occur);
			}
			else
			{
				var parser = new QueryParser(Consts.LuceneVersion, indexName, new StandardAnalyzer(Consts.LuceneVersion));
				var query = parser.Parse($"ISNULL:{indexName}");
			}
			return instance;
		}

		public static BooleanQuery AddNumericRangeQuery<TObj>(
			this BooleanQuery instance,
			Expression<Func<TObj, int>> expression,
			int lowerInclusive,
			int upperInclusive,
			Occur occur)
		{
			string indexName = ExpressionExtensions.GetIndexName(expression);
			var query = NumericRangeQuery.NewInt32Range(indexName, lowerInclusive, upperInclusive, true, true);
			instance.Add(query, occur);
			return instance;
		}

		public static BooleanQuery AddNumericRangeQuery<TObj>(
			this BooleanQuery instance,
			Expression<Func<TObj, int?>> expression,
			int lowerInclusive,
			int upperInclusive,
			Occur occur)
		{
			string indexName = ExpressionExtensions.GetIndexName(expression);
			var query = NumericRangeQuery.NewInt32Range(indexName, lowerInclusive, upperInclusive, true, true);
			instance.Add(query, occur);
			return instance;
		}

	}
}
