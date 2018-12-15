using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;

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

			var document = new Document();
			document.AddObject(entry);
			document.StoreAndIndex(entry, x => x.DictionaryCode);
			document.StoreAndIndex(entry, x => x.Code);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
