using System;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;

namespace QuranX.Persistence.Services.Lucene
{
	public interface ILuceneIndexWriterProvider
	{
		IndexWriter GetIndexWriter();
	}

	public class LuceneIndexWriterProvider : ILuceneIndexWriterProvider, IDisposable
	{
		private static readonly object SyncRoot = new object();
		private static ILuceneDirectoryProvider LuceneDirectoryProvider;
		private static Analyzer Analyzer;
		private static Lazy<IndexWriter> IndexWriter;

		public LuceneIndexWriterProvider(ILuceneDirectoryProvider luceneDirectoryProvider, Analyzer analyzer)
		{
			if (LuceneDirectoryProvider == null)
			{
				lock (SyncRoot)
				{
					if (LuceneDirectoryProvider == null)
					{
						LuceneDirectoryProvider = luceneDirectoryProvider;
						Analyzer = analyzer;

						IndexWriter = new Lazy<IndexWriter>(() =>
						{
							var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, Analyzer) {
								OpenMode = OpenMode.CREATE_OR_APPEND
							};
							return new IndexWriter(LuceneDirectoryProvider.GetDirectory(), indexWriterConfig);
						});
					}
				}
			}
		}

		public IndexWriter GetIndexWriter()
		{
			return IndexWriter.Value;
		}

		public void Dispose()
		{
			if (IndexWriter != null && IndexWriter.IsValueCreated)
			{
				IndexWriter.Value.Dispose();
			}

			if (LuceneDirectoryProvider != null)
			{
				LuceneDirectoryProvider.GetDirectory().Dispose();
			}
		}
	}
}
