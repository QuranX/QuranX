using System;
using Lucene.Net.Index;

namespace QuranX.Persistence.Services
{
	public interface ILuceneIndexWriterProvider
	{
		IndexWriter GetIndexWriter();
	}

	public class LuceneIndexWriterProvider : ILuceneIndexWriterProvider
	{
		private readonly Lazy<IndexWriter> _indexWriter;

		public LuceneIndexWriterProvider(
			ILuceneDirectoryProvider directoryProvider, 
			ILuceneAnalyzerProvider analyzerProvider)
		{
			_indexWriter = new Lazy<IndexWriter>(() => new IndexWriter(directoryProvider.GetDirectory(), analyzerProvider.GetAnalyzer(), IndexWriter.MaxFieldLength.UNLIMITED));
		}

		public IndexWriter GetIndexWriter()
		{
			return _indexWriter.Value;
		}
	}
}
