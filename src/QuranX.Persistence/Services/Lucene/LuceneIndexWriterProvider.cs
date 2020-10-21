using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Index;

namespace QuranX.Persistence.Services.Lucene
{
	public interface ILuceneIndexWriterProvider
	{
		IndexWriter GetIndexWriter();
	}


	public class LuceneIndexWriterProvider : ILuceneIndexWriterProvider
	{
		private static object _syncRoot = new Object();
		private static ILuceneDirectoryProvider _luceneDirectoryProvider;
		private static Analyzer _analyzer;
		private static Lazy<IndexWriter> _indexWriter =
			new Lazy<IndexWriter>(() => new IndexWriter(
				_luceneDirectoryProvider.GetDirectory(),
				_analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED));

		public LuceneIndexWriterProvider(ILuceneDirectoryProvider luceneDirectoryProvider, Analyzer analyzer)
		{
			if (_luceneDirectoryProvider == null)
			{
				lock (_syncRoot)
				{
					_luceneDirectoryProvider = _luceneDirectoryProvider ?? luceneDirectoryProvider;
					_analyzer = _analyzer ?? analyzer;
				}
			}
		}

		public IndexWriter GetIndexWriter()
		{
			return _indexWriter.Value;
		}
	}
}
