using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IDictionaryRepository
	{
		IEnumerable<Dictionary> GetAll();
		Dictionary Get(string dictionaryCode);
	}

	public class DictionaryRepository : IDictionaryRepository
	{
		private readonly object SyncRoot = new object();
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;
		private Dictionary<string, Dictionary> DictionariesByCode;
		private Dictionary[] Dictionaries;

		public DictionaryRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public Dictionary Get(string dictionaryCode)
		{
			EnsureData();
			return DictionariesByCode[dictionaryCode];
		}

		public IEnumerable<Dictionary> GetAll()
		{
			EnsureData();
			return Dictionaries;
		}

		private void EnsureData()
		{
			if (DictionariesByCode == null)
			{
				lock (SyncRoot)
				{
					if (DictionariesByCode == null)
					{
						Dictionaries = GetData().OrderBy(x => x.Code).ToArray();
						DictionariesByCode = Dictionaries.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);
					}
				}
			}
		}

		private Dictionary[] GetData()
		{
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<Dictionary>();

			IndexSearcher indexSeacher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = indexSeacher.Search(query, 1000);
			Dictionary[] dictionaries = docs.ScoreDocs
				.Select(x => x.Doc)
				.Distinct()
				.Select(docId => indexSeacher.Doc(docId).GetObject<Dictionary>())
				.OrderBy(x => x.Code)
				.ToArray();
			return dictionaries;
		}

	}
}
