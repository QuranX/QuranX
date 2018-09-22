using System.Linq;
using Lucene.Net.Index;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentatorRepository
	{
		Commentator[] GetAll();
	}

	public class CommentatorRepository : ICommentatorRepository
	{
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;

		public CommentatorRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public Commentator[] GetAll()
		{
			IndexSearcher searcher = IndexSearcherProvider.GetIndexSearcher();
			var query = new BooleanQuery(disableCoord: true);
			query.FilterByType<Commentator>();

			IndexSearcher indexSeacher = IndexSearcherProvider.GetIndexSearcher();
			TopDocs docs = indexSeacher.Search(query, 1000);
			Commentator[] commentators = docs.ScoreDocs
				.Select(x => x.Doc)
				.Distinct()
				.Select(docId => indexSeacher.Doc(docId).GetObject<Commentator>())
				.OrderBy(x => x.Description)
				.ToArray();
			return commentators;
		}
	}
}
