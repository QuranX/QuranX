using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentatorWriteRepository
	{
		void Write(Commentator commentator);
	}

	public class CommentatorWriteRepository : ICommentatorWriteRepository
	{
		private ILuceneIndexWriterProvider IndexWriterProvider;

		public CommentatorWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
		{
			IndexWriterProvider = indexWriterProvider;
		}

		public void Write(Commentator commentator)
		{
			if (commentator == null)
				throw new ArgumentNullException(nameof(commentator));

			var document = new Document();
			document.AddIndexed(nameof(Commentator.Code), commentator.Code);
			document.AddObject(commentator);
			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.AddDocument(document);
		}
	}
}
