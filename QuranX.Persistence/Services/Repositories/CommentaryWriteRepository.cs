using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentaryWriteRepository
	{
		void Write(Commentary commentary);
	}

	public class CommentaryWriteRepository : ICommentaryWriteRepository
	{
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;

		public CommentaryWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Commentary commentary)
		{
			if (commentary == null)
				throw new ArgumentNullException(nameof(commentary));

			var document = new Document();
			document.AddIndexed(nameof(Commentary.CommentatorCode), commentary.CommentatorCode);
			document.AddIndexed(nameof(Commentary.ChapterNumber), commentary.ChapterNumber);
			document.AddIndexed(nameof(Commentary.FirstVerseNumber), commentary.FirstVerseNumber);
			document.AddIndexed(nameof(Commentary.LastVerseNumber), commentary.LastVerseNumber);
			document.AddFullText(commentary.Text);
			document.AddObject(commentary);

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
