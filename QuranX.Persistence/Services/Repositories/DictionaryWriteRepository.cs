using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IDictionaryWriteRepository
	{
		void Write(Dictionary dictionary);
	}

	public class DictionaryWriteRepository : IDictionaryWriteRepository
	{
		private ILuceneIndexWriterProvider IndexWriterProvider;

		public DictionaryWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Dictionary dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			var document = new Document();
			document.AddObject(dictionary);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
