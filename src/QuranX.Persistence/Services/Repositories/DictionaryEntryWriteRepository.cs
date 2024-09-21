using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Shared;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IDictionaryEntryWriteRepository
	{
		void Write(Models.DictionaryEntry entry);
	}

	public class DictionaryEntryWriteRepository : IDictionaryEntryWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public DictionaryEntryWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Models.DictionaryEntry entry)
		{
			if (entry == null)
				throw new ArgumentNullException(nameof(entry));

			string indexValue = ArabicWordIndexer.GetIndexForArabic(entry.Word);
			var document = new Document();
			document.AddObject(entry);
			document.StoreAndIndex(entry, x => x.DictionaryCode);
			document.StoreAndIndex(entry, x => x.Word, ArabicHelper.SubstituteAndOmit);
			document.StoreAndIndex(DictionaryEntryRepository.RootWordsIndexName, indexValue);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
