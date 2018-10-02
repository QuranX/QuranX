using System;
using System.Linq.Expressions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Persistence.Extensions
{
	public static class BooleanQueryExtensions
	{
		public static BooleanQuery FilterByType<T>(this BooleanQuery instance)
		{
			var term = new Term(Consts.SerializedObjectTypeFieldName, typeof(T).Name.ToLowerInvariant());
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
			if (value != null)
			{
				value = value.ToLowerInvariant();

				var term = new Term(indexName, value);
				var subQuery = new TermQuery(term);
				instance.Add(subQuery, occur);
			}
			else
			{
				var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, indexName, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
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
			var query = NumericRangeQuery.NewIntRange(indexName, lowerInclusive, upperInclusive, true, true);
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
			var query = NumericRangeQuery.NewIntRange(indexName, lowerInclusive, upperInclusive, true, true);
			instance.Add(query, occur);
			return instance;
		}

	}
}
