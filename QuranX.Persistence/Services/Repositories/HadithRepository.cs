using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IHadithRepository
	{
		IEnumerable<HadithReference> GetReferences(
			string collectionCode,
			string indexCode,
			IEnumerable<(int index, string suffix)> values);
	}

	public class HadithRepository : IHadithRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public HadithRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public IEnumerable<HadithReference> GetReferences(
			string collectionCode,
			string indexCode,
			IEnumerable<(int index, string suffix)> values)
		{
			IEnumerable<int> docIds = GetReferencesIds(
				collectionCode: collectionCode,
				indexCode: indexCode,
				values: values);
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			IEnumerable<HadithReference> references = docIds
				.Select(x => searcher.Doc(x).GetObject<HadithReference>());
			return references;
		}

		public IEnumerable<int> GetReferencesIds(
		string collectionCode,
		string indexCode,
		IEnumerable<(int index, string suffix)> values)
		{
			values = values ?? Array.Empty<(int index, string suffix)>();

			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<HadithReference>()
				.AddPhraseQuery<HadithReference>(x => x.CollectionCode, collectionCode, Occur.MUST)
				.AddPhraseQuery<HadithReference>(x => x.IndexCode, indexCode.Replace("-", ""), Occur.MUST);
			(int index, string suffix)[] valuesArray = values.ToArray();
			if (valuesArray.Length > 0)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.IndexPart1,
					valuesArray[0].index,
					valuesArray[0].index,
					Occur.MUST);
				query.AddPhraseQuery<HadithReference>(x =>
					x.IndexPart1Suffix,
					valuesArray[0].suffix,
					Occur.MUST);
			}
			if (valuesArray.Length > 1)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.IndexPart1,
					valuesArray[1].index,
					valuesArray[1].index,
					Occur.MUST);
				query.AddPhraseQuery<HadithReference>(x =>
					x.IndexPart1Suffix,
					valuesArray[1].suffix,
					Occur.MUST);
			}
			if (valuesArray.Length > 2)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.IndexPart1,
					valuesArray[2].index,
					valuesArray[2].index,
					Occur.MUST);
				query.AddPhraseQuery<HadithReference>(x =>
					x.IndexPart1Suffix,
					valuesArray[2].suffix,
					Occur.MUST);
			}
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 99000);
			IEnumerable<int> hadithReferences = docs.ScoreDocs.Select(x => x.Doc).ToArray();
			return hadithReferences;
		}
	}
}
