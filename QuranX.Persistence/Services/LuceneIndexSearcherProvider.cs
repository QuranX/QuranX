using System;
using Lucene.Net.Search;

namespace QuranX.Persistence.Services
{
	public interface ILuceneIndexSearcherProvider
	{
		IndexSearcher GetIndexSearcher();
	}

	public class LuceneIndexSearcherProvider : ILuceneIndexSearcherProvider
	{
		private readonly Lazy<IndexSearcher> _indexSearcher;

		public LuceneIndexSearcherProvider(ILuceneIndexReaderProvider indexReaderProvider)
		{
			_indexSearcher = new Lazy<IndexSearcher>(() => new IndexSearcher(indexReaderProvider.GetReader()));
		}

		public IndexSearcher GetIndexSearcher()
		{
			return _indexSearcher.Value;
		}
	}
}

