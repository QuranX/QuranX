using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.WebTesting;

namespace QuranX.Web.LoadTest.WebTestPlugins
{
	[DisplayName("Enable browser caching")]
	public class EnableBrowserCachingPlugin : WebTestPlugin
	{
		[DisplayName("Allow caching")]
		[Description("If True then server responses will be cached")]
		public bool AllowCaching { get; set; } = true;

		public override void PostRequest(object sender, PostRequestEventArgs e)
		{
			foreach (WebTestRequest dependentRequest in e.Request.DependentRequests)
			{
				dependentRequest.Cache = AllowCaching;
			}
		}
	}
}
