using System;
using Lucene.Net.Index;

namespace QuranX.Persistence.Services
{
	public interface ILuceneIndexReaderProvider
	{
		IndexReader GetReader();
	}

	public class LuceneIndexReaderProvider : ILuceneIndexReaderProvider
	{
		private readonly Lazy<IndexReader> IndexReader;

		public LuceneIndexReaderProvider(ILuceneDirectoryProvider directoryProvider)
		{
			IndexReader = new Lazy<IndexReader>(() => Lucene.Net.Index.IndexReader.Open(directoryProvider.GetDirectory(), true));
		}

		public IndexReader GetReader()
		{
			return IndexReader.Value;
		}
	}
}
