using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Models;
using QuranX.Persistence.Extensions;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories
{
	public interface IVerseWriterRepository
	{
		void Write(Verse verse);
	}

	public class VerseWriterRepository : IVerseWriterRepository
	{
		private readonly ILuceneIndexWriterProvider _indexWriterProvider;

		public VerseWriterRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			_indexWriterProvider = indexWriterProvider;
		}

		public void Write(Verse verse)
		{
			if (verse == null)
				throw new ArgumentNullException(nameof(verse));

			var document = new Document();
			document
				.AddIndexed(nameof(verse.ChapterNumber), verse.ChapterNumber)
				.AddIndexed(nameof(verse.VerseNumber), verse.VerseNumber)
				.AddFullText(verse.VerseTexts.Select(x => x.Text))
				.AddObject(verse);
			IndexWriter indexWriter = _indexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
