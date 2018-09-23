using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IHadithCollectionRepository
	{
		HadithCollection[] GetAll();
		HadithCollection Get(string collectionCode);
	}

	public class HadithCollectionRepository : IHadithCollectionRepository
	{
		private readonly object SyncRoot = new object();
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;
		private HadithCollection[] HadithCollections;
		private Dictionary<string, HadithCollection> HadithCollectionByCode;

		public HadithCollectionRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public HadithCollection Get(string collectionCode)
		{
			EnsureData();
			return HadithCollectionByCode[collectionCode];
		}

		public HadithCollection[] GetAll()
		{
			EnsureData();
			return HadithCollections;
		}

		private void EnsureData()
		{
			if (HadithCollectionByCode == null)
			{
				lock (SyncRoot)
				{
					if (HadithCollectionByCode == null)
					{
						HadithCollections = GetData().OrderBy(x => x.Code).ToArray();
						HadithCollectionByCode = HadithCollections.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);
					}
				}
			}
		}

		private HadithCollection[] GetData()
		{
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<HadithCollection>();

			IndexSearcher indexSeacher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = indexSeacher.Search(query, 1000);
			HadithCollection[] HadithCollections = docs.ScoreDocs
				.Select(x => x.Doc)
				.Distinct()
				.Select(docId => indexSeacher.Doc(docId).GetObject<HadithCollection>())
				.OrderBy(x => x.Name)
				.ToArray();
			return HadithCollections;
		}
	}
}
