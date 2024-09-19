using System;
using Lucene.Net.Index;

namespace QuranX.Persistence.Services
{
	public interface ILuceneIndexReaderProvider
	{
		IndexReader GetReader();
	}

	public class LuceneIndexReaderProvider : ILuceneIndexReaderProvider, IDisposable
	{
		private readonly Lazy<DirectoryReader> IndexReader;
		private readonly ILuceneDirectoryProvider DirectoryProvider;
		private bool disposedValue;

		public LuceneIndexReaderProvider(ILuceneDirectoryProvider directoryProvider)
		{
			DirectoryProvider = directoryProvider;
			IndexReader = new Lazy<DirectoryReader>(() => DirectoryReader.Open(DirectoryProvider.GetDirectory()));
		}

		public IndexReader GetReader()
		{
			return IndexReader.Value;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing && IndexReader.IsValueCreated)
				{
					IndexReader.Value.Dispose();
					DirectoryProvider.GetDirectory().Dispose();
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
