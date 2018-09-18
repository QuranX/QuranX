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
		private readonly Lazy<IndexReader> _indexReader;

		public LuceneIndexReaderProvider(ILuceneDirectoryProvider directoryProvider)
		{
			_indexReader = new Lazy<IndexReader>(() => IndexReader.Open(directoryProvider.GetDirectory(), true));
		}

		public IndexReader GetReader()
		{
			return _indexReader.Value;
		}
	}
}
