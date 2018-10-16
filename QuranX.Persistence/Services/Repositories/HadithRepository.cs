using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IHadithRepository
	{
		IEnumerable<HadithReference> GetReferences(
			string collectionCode,
			string referenceCode,
			IEnumerable<(int value, string suffix)> values);
		IEnumerable<Hadith> GetHadiths(IEnumerable<int> ids);
		IEnumerable<Hadith> GetForVerse(VerseReference verseReference);
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
			string referenceCode,
			IEnumerable<(int value, string suffix)> values)
		{
			IEnumerable<int> docIds = GetReferencesIds(
				collectionCode: collectionCode,
				referenceCode: referenceCode,
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
			string referenceCode,
			IEnumerable<(int value, string suffix)> values)
		{
			values = values ?? Array.Empty<(int value, string suffix)>();
			HadithCollection collection = HadithCollectionRepository.Get(collectionCode);
			HadithReferenceDefinition referenceDefinition = collection.GetReferenceDefinition(referenceCode);

			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<HadithReference>()
				.AddPhraseQuery<HadithReference>(x => x.CollectionCode, collectionCode, Occur.MUST)
				.AddPhraseQuery<HadithReference>(x => x.ReferenceCode, referenceCode.Replace("-", ""), Occur.MUST);
			(int value, string suffix)[] valuesArray = values.ToArray();

			Func<int, bool> shouldFilterOnSuffix = referencePartNumber =>
			{
				// If not the last reference value then we always filter
				// on the suffix. This includes ensuring the suffix is null.
				if (referenceDefinition.PartNames.Count != referencePartNumber)
					return true;
				// If it is the last reference value then we only filter
				// on suffix if the value filtering by is not null.
				return !string.IsNullOrEmpty(valuesArray[referencePartNumber - 1].suffix);
			};

			if (valuesArray.Length > 0)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.ReferenceValue1,
					valuesArray[0].value,
					valuesArray[0].value,
					Occur.MUST);
				if (shouldFilterOnSuffix(1))
				{
					query.AddPhraseQuery<HadithReference>(x =>
						x.ReferenceValue1Suffix,
						valuesArray[0].suffix.AsNullIfWhiteSpace(),
						Occur.MUST);
				}
			}
			if (valuesArray.Length > 1)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.ReferenceValue2,
					valuesArray[1].value,
					valuesArray[1].value,
					Occur.MUST);
				if (shouldFilterOnSuffix(2))
				{
					query.AddPhraseQuery<HadithReference>(x =>
					x.ReferenceValue2Suffix,
					valuesArray[1].suffix.AsNullIfWhiteSpace(),
					Occur.MUST);
				}
			}
			if (valuesArray.Length > 2)
			{
				query.AddNumericRangeQuery<HadithReference>(x =>
					x.ReferenceValue3,
					valuesArray[2].value,
					valuesArray[2].value,
					Occur.MUST);
				if (shouldFilterOnSuffix(3))
				{
					query.AddPhraseQuery<HadithReference>(x =>
					x.ReferenceValue3Suffix,
					valuesArray[2].suffix.AsNullIfWhiteSpace(),
					Occur.MUST);
				}
			}
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 99000);
			IEnumerable<int> hadithReferences = docs.ScoreDocs.Select(x => x.Doc).ToArray();
			return hadithReferences;
		}

		public IEnumerable<Hadith> GetForVerse(VerseReference verseReference)
		{
			int verseIndexValue = verseReference.ToIndexValue();
			var query = new BooleanQuery(disableCoord: true)
				.FilterByType<HadithVerseLink>()
				.AddNumericRangeQuery<HadithVerseLink>(x => x.VerseId, verseIndexValue, verseIndexValue, Occur.MUST);
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 99000);
			IEnumerable<int> hadithIds = docs.ScoreDocs.Select(x => searcher.Doc(x.Doc).GetStoredValue<HadithVerseLink>(i => i.HadithId));
			return GetHadiths(hadithIds);
		}
	}

	internal class HadithVerseLink
	{
		public int HadithId { get; }
		public int VerseId { get; }

		public HadithVerseLink(int hadithId, int verseId)
		{
			HadithId = hadithId;
			VerseId = verseId;
		}
	}
}
