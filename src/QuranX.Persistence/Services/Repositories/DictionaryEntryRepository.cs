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
		IEnumerable<string> GetNextRoots(string parentRoot);
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
				.AddStringEqualsQuery(RootWordsIndexName, indexValue, Occur.MUST);

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
				.AddStringEqualsQuery<DictionaryEntry>(x => x.DictionaryCode, dictionaryCode, Occur.MUST)
				.AddStringEqualsQuery(RootWordsIndexName, indexValue, Occur.MUST);

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 7000);
			IEnumerable<DictionaryEntry> results = docs.ScoreDocs
				.Select(x => searcher.Doc(x.Doc))
				.Select(x => x.GetObject<DictionaryEntry>())
				.OrderBy(x => x.EntryIndex);
			return results;
		}

		public IEnumerable<string> GetNextRoots(string root)
		{
			string[] roots;
			if (string.IsNullOrWhiteSpace(root))
			{
				roots = ArabicAlphabet.Letters.Select(x => x.ToString()).ToArray();
			}
			else
			{
				var query = new BooleanQuery(disableCoord: true);
				query
					.FilterByType<DictionaryEntry>()
					.AddStringStartsWithQuery<DictionaryEntry>(x => x.Word, root, Occur.MUST);

				int nextRootLength = root.Length + 1;
				IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
				TopDocs docs = searcher.Search(query, 7000);
				roots = docs.ScoreDocs
					.Select(x => searcher.Doc(x.Doc))
					.Select(x => x.GetStoredValue<DictionaryEntry>(x => x.Word))
					.Where(x => x.Length >= nextRootLength)
					.Select(x => x.Substring(0, nextRootLength))
					.Distinct()
					.OrderBy(x => x)
					.ToArray();
			}
			return roots;
		}
	}
}
