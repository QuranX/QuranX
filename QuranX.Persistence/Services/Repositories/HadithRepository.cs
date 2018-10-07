using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
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
		IEnumerable<Hadith> GetHadiths(IEnumerable<int> ids);
	}

	public class HadithRepository : IHadithRepository
	{
		private readonly IHadithCollectionRepository HadithCollectionRepository;
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public HadithRepository(
			IHadithCollectionRepository hadithCollectionRepository,
			ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			HadithCollectionRepository = hadithCollectionRepository;
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

		public IEnumerable<Hadith> GetHadiths(IEnumerable<int> ids)
		{
			if (ids == null || !ids.Any())
				return Array.Empty<Hadith>();

			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<Hadith>();
			var idQuery = new BooleanQuery(disableCoord: true);
			foreach (int id in ids)
			{
				idQuery.AddNumericRangeQuery<Hadith>(x => x.Id, id, id, Occur.SHOULD);
			}
			query.Add(new BooleanClause(idQuery, Occur.MUST));

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 99000);
			IEnumerable<int> documentIds = docs.ScoreDocs.Select(x => x.Doc);
			IEnumerable<Document> documents = documentIds.Select(x => searcher.Doc(x));
			Dictionary<int, Hadith> hadithsById =
				documents
				.Select(x => x.GetObject<Hadith>())
				.ToDictionary(x => x.Id);

			// Return objects in ID order
			return ids.Select(x => hadithsById[x]);
		}

		private IEnumerable<int> GetReferencesIds(
			string collectionCode,
			string indexCode,
			IEnumerable<(int index, string suffix)> values)
		{
			values = values ?? Array.Empty<(int index, string suffix)>();
			HadithCollection collection = HadithCollectionRepository.Get(collectionCode);
			HadithReferenceDefinition indexDefinition = collection.GetReferenceDefinition(indexCode);

			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<HadithReference>()
				.AddPhraseQuery<HadithReference>(x => x.CollectionCode, collectionCode, Occur.MUST)
				.AddPhraseQuery<HadithReference>(x => x.IndexCode, indexCode.Replace("-", ""), Occur.MUST);
			(int index, string suffix)[] valuesArray = values.ToArray();

			Func<int, bool> shouldFilterOnSuffix = indexPartNumber =>
			{
				// If not the last part of the index then we always filter
				// on the suffix. This includes ensuring the suffix is null.
				if (indexDefinition.PartNames.Count != indexPartNumber)
					return true;
				// If it is the last part of the index then we only filter
				// on suffix if the value filtering by is not null.
				return !string.IsNullOrEmpty(valuesArray[indexPartNumber - 1].suffix);
			};

			if (valuesArray.Length > 0)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.IndexPart1,
					valuesArray[0].index,
					valuesArray[0].index,
					Occur.MUST);
				if (shouldFilterOnSuffix(1))
				{
					query.AddPhraseQuery<HadithReference>(x =>
						x.IndexPart1Suffix,
						valuesArray[0].suffix.AsNullIfWhiteSpace(),
						Occur.MUST);
				}
			}
			if (valuesArray.Length > 1)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.IndexPart2,
					valuesArray[1].index,
					valuesArray[1].index,
					Occur.MUST);
				if (shouldFilterOnSuffix(2))
				{
					query.AddPhraseQuery<HadithReference>(x =>
					x.IndexPart2Suffix,
					valuesArray[1].suffix.AsNullIfWhiteSpace(),
					Occur.MUST);
				}
			}
			if (valuesArray.Length > 2)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.IndexPart3,
					valuesArray[2].index,
					valuesArray[2].index,
					Occur.MUST);
				if (shouldFilterOnSuffix(3))
				{
					query.AddPhraseQuery<HadithReference>(x =>
					x.IndexPart3Suffix,
					valuesArray[2].suffix.AsNullIfWhiteSpace(),
					Occur.MUST);
				}
			}
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 99000);
			IEnumerable<int> hadithReferences = docs.ScoreDocs.Select(x => x.Doc).ToArray();
			return hadithReferences;
		}
	}
}
