using Lucene.Net.Index;
using Lucene.Net.Search;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Persistence.Extensions
{
	public static class BooleanQueryExtensions
	{
		public static void FilterByType<T>(this BooleanQuery instance)
		{
			var term = new Term(Consts.SerializedObjectTypeFieldName, typeof(T).Name.ToLower());
			var query = new PhraseQuery();
			query.Add(term);
			instance.Add(query, Occur.MUST);
		}
	}
}
