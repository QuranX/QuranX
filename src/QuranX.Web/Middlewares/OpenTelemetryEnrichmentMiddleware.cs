using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
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
		Activity currentActivity = Activity.Current;
		if (currentActivity != null)
		{
			// Add route data to activity tags
			RouteData routeData = httpContext.GetRouteData();
			foreach (KeyValuePair<string, object> routeValue in routeData.Values)
				currentActivity.SetTag($"http.route.{routeValue.Key}", routeValue.Value?.ToString());

			// Add query string values to activity tags
			IQueryCollection queryString = httpContext.Request.Query;
			foreach (KeyValuePair<string, StringValues> query in queryString)
				currentActivity.SetTag($"http.query.{query.Key}", query.Value.ToString());
		}

		await Next(httpContext);
	}
}
