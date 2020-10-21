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
		private readonly Lazy<IndexWriter> IndexWriter;

		public LuceneIndexWriterProvider(
			ILuceneDirectoryProvider directoryProvider, 
			ILuceneAnalyzerProvider analyzerProvider)
		{
			IndexWriter = new Lazy<IndexWriter>(() =>
			{
				return new IndexWriter(
					directoryProvider.GetDirectory(), 
					analyzerProvider.GetAnalyzer(), 
					Lucene.Net.Index.IndexWriter.MaxFieldLength.UNLIMITED);
			});
		}

		public IndexWriter GetIndexWriter()
		{
			return IndexWriter.Value;
		}
	}
}
