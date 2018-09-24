using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Models;
using QuranX.Persistence.Extensions;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseWriteRepository
	{
		void Write(Verse verse);
	}

	public class VerseWriteRepository : IVerseWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public VerseWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Verse verse)
		{
			if (verse == null)
				throw new ArgumentNullException(nameof(verse));

			var document = new Document();
			document
				.StoreAndIndex(verse, x => x.Id)
				.StoreAndIndex(verse, x => x.ChapterNumber)
				.StoreAndIndex(verse, x => x.VerseNumber)
				.AddSearchableText(verse.VerseTexts.Select(x => x.Text))
				.AddObject(verse);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
