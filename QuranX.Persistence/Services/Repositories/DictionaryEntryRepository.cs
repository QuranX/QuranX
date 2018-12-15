using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IDictionaryEntryRepository
	{
		DictionaryEntry Get(string dictionaryCode, string code);
	}

	public class DictionaryEntryRepository : IDictionaryEntryRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public DictionaryEntryRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public DictionaryEntry Get(string dictionaryCode, string code)
		{
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<DictionaryEntry>();
			query
				.AddPhraseQuery<DictionaryEntry>(x => x.DictionaryCode, dictionaryCode, Occur.MUST)
				.AddPhraseQuery<DictionaryEntry>(x => x.Code, code, Occur.MUST);

			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = searcher.Search(query, 1);
			if (docs.TotalHits == 0)
				return null;
			if (docs.TotalHits > 1)
				throw new InvalidOperationException($"Too many results for {dictionaryCode} {code}");
			return searcher.Doc(docs.ScoreDocs[0].Doc).GetObject<DictionaryEntry>();
		}
	}
}
