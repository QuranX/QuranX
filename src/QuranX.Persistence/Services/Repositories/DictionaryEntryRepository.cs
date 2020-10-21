using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IDictionaryEntryRepository
	{
		IEnumerable<DictionaryEntry> Get(string word);
		IEnumerable<DictionaryEntry> Get(string dictionaryCode, string word);
	}

	public class DictionaryEntryRepository : IDictionaryEntryRepository
	{
		public const string RootWordsIndexName = nameof(DictionaryEntry) + "_WordIndex";
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public DictionaryEntryRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public IEnumerable<DictionaryEntry> Get(string word)
		{
			string indexValue = ArabicWordIndexer.GetIndexForArabic(word);
			var query = new BooleanQuery(disableCoord: true);
			query
				.FilterByType<DictionaryEntry>()
				.AddPhraseQuery(RootWordsIndexName, indexValue, Occur.MUST);

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			IEnumerable<DictionaryEntry> results = docs.ScoreDocs
				.Select(x => searcher.Doc(x.Doc))
				.Select(x => x.GetObject<DictionaryEntry>());
			return results;
		}

		public IEnumerable<DictionaryEntry> Get(string dictionaryCode, string word)
		{
			string indexValue = ArabicWordIndexer.GetIndexForArabic(word);
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<DictionaryEntry>();
			query
				.AddPhraseQuery<DictionaryEntry>(x => x.DictionaryCode, dictionaryCode, Occur.MUST)
				.AddPhraseQuery(RootWordsIndexName, indexValue, Occur.MUST);

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			IEnumerable<DictionaryEntry> results = docs.ScoreDocs
				.Select(x => searcher.Doc(x.Doc))
				.Select(x => x.GetObject<DictionaryEntry>())
				.OrderBy(x => x.EntryIndex);
			return results;
		}
	}
}
