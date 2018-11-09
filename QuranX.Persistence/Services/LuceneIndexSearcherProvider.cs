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
		private readonly Lazy<IndexSearcher> IndexSearcher;

		public LuceneIndexSearcherProvider(ILuceneIndexReaderProvider indexReaderProvider)
		{
			IndexSearcher = new Lazy<IndexSearcher>(() => new IndexSearcher(indexReaderProvider.GetReader()));
		}

		public IndexSearcher GetIndexSearcher()
		{
			return IndexSearcher.Value;
		}
	}
}

