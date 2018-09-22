using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Search;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentatorRepository
	{
		Commentator[] GetAll();
		Commentator Get(string commentatorCode);
	}

	public class CommentatorRepository : ICommentatorRepository
	{
		private readonly object SyncRoot = new object();
		private readonly ILuceneIndexSearcherProvider IndexSearcherProvider;
		private Commentator[] Commentators;
		private Dictionary<string, Commentator> CommentatorByCode;

		public CommentatorRepository(ILuceneIndexSearcherProvider indexSearcherProvider)
		{
			IndexSearcherProvider = indexSearcherProvider;
		}

		public Commentator Get(string commentatorCode)
		{
			EnsureData();
			return CommentatorByCode[commentatorCode];
		}

		public Commentator[] GetAll()
		{
			EnsureData();
			return Commentators;
		}

		private void EnsureData()
		{
			if (CommentatorByCode == null)
			{
				lock (SyncRoot)
				{
					if (CommentatorByCode == null)
					{
						Commentators = GetData().OrderBy(x => x.Code).ToArray();
						CommentatorByCode = Commentators.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);
					}
				}
			}
		}

		private Commentator[] GetData()
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
