using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace QuranX.Web.Middlewares;

public class OpenTelemetryEnrichmentMiddleware
{
	private readonly RequestDelegate Next;

	public OpenTelemetryEnrichmentMiddleware(RequestDelegate next)
	{
		Next = next;
	}

	public async Task InvokeAsync(HttpContext httpContext)
	{
		Activity currentActivity = System.Diagnostics.Activity.Current;
		if (currentActivity != null)
		{
			RouteData routeData = httpContext.GetRouteData();
			foreach (KeyValuePair<string, object> routeValue in routeData.Values)
				currentActivity.SetTag($"http.route.{routeValue.Key}", routeValue.Value?.ToString());
		}

		await Next(httpContext);
	}
}
