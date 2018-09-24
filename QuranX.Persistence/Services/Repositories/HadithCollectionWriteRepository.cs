using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IHadithCollectionWriteRepository
	{
		void Write(HadithCollection collection);
	}

	public class HadithCollectionWriteRepository : IHadithCollectionWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public HadithCollectionWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(HadithCollection collection)
		{
			var document = new Document();
			document.Index(collection, x => x.Code);
			document.AddObject(collection);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
