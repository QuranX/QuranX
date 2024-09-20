using System;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace QuranX.Persistence.Services
{
	public interface ILuceneIndexWriterProvider
	{
		IndexWriter GetIndexWriter();
	}

	public class LuceneIndexWriterProvider : ILuceneIndexWriterProvider, IDisposable
	{
		private readonly Lazy<IndexWriter> indexWriter;
		private readonly ILuceneDirectoryProvider directoryProvider;
		private readonly ILuceneAnalyzerProvider analyzerProvider;
		private bool disposedValue;

		public LuceneIndexWriterProvider(
			ILuceneDirectoryProvider directoryProvider,
			ILuceneAnalyzerProvider analyzerProvider)
		{
			this.directoryProvider = directoryProvider;
			this.analyzerProvider = analyzerProvider;

			indexWriter = new Lazy<IndexWriter>(() =>
			{
				Analyzer analyzer = analyzerProvider.GetAnalyzer();
				var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer) {
					OpenMode = OpenMode.CREATE_OR_APPEND
					// You can set additional configurations here if needed
				};

				return new IndexWriter(directoryProvider.GetDirectory(), indexWriterConfig);
			});
		}

		public IndexWriter GetIndexWriter()
		{
			return indexWriter.Value;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing && indexWriter.IsValueCreated)
				{
					indexWriter.Value.Dispose();
					directoryProvider.GetDirectory().Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
