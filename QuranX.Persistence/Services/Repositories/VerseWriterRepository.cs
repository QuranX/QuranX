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
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public VerseWriterRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Verse verse)
		{
			if (verse == null)
				throw new ArgumentNullException(nameof(verse));

			var document = new Document();
			document
				.AddIndexed(nameof(Verse.ID), verse.ID)
				.AddIndexed(nameof(Verse.ChapterNumber), verse.ChapterNumber)
				.AddIndexed(nameof(Verse.VerseNumber), verse.VerseNumber)
				.AddFullText(verse.VerseTexts.Select(x => x.Text))
				.AddObject(verse);
			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
