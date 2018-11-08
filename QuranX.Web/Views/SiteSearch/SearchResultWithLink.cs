namespace QuranX.Web.Views.Search
{
	public class SearchResultWithLink
	{
		public readonly string Url;
		public readonly string Caption;
		public readonly string[] Snippets;

		public SearchResultWithLink(string url, string caption, string[] snippets)
		{
			Url = url;
			Caption = caption;
			Snippets = snippets;
		}
	}
}