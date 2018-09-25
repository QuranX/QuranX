using System;
using System.Linq.Expressions;
using Lucene.Net.Index;
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
			if (value != null)
				value = value.ToLowerInvariant();

			string indexName = ExpressionExtensions.GetIndexName(expression);
			var term = new Term(indexName, value);
			var subQuery = new PhraseQuery();
			subQuery.Add(term);
			instance.Add(subQuery, occur);
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

	}
}
