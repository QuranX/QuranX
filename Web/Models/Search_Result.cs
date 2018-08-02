using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuranX.Models
{
	public class Search_Result
	{
		public string Type { get; set; }
		public string ID { get; set; }
		public string Caption { get; set; }
		public string Url { get; set; }
		public string[] Snippets { get; set; }

		public Search_Result(string type, string id, string caption, string url, string[] snippets)
		{
			this.Type = type;
			this.ID = id;
			this.Caption = caption;
			this.Url = url;
			this.Snippets = snippets;
		}
	}
}