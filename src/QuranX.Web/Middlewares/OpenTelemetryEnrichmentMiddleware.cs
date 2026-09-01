using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuranX.Web.Middlewares;

public class OpenTelemetryEnrichmentMiddleware
{
    private const int MaxQueryPairs = 20;
    private const int MaxQueryValueLength = 200;

    private readonly RequestDelegate Next;

    public OpenTelemetryEnrichmentMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        Activity currentActivity = Activity.Current;
        if (currentActivity is not null)
        {
            // Add route data to activity tags
            RouteData routeData = httpContext.GetRouteData();
            foreach (KeyValuePair<string, object> routeValue in routeData.Values)
                currentActivity.SetTag($"http.route.{routeValue.Key}", routeValue.Value?.ToString());

            // Add query string values to a single tag. The keys are caller-controlled, so using
            // them as tag names would create unbounded attribute cardinality.
            IQueryCollection queryString = httpContext.Request.Query;
            if (queryString.Count > 0)
            {
                string[] queryPairs = queryString
                    .Take(MaxQueryPairs)
                    .Select(x => $"{x.Key}={Truncate(x.Value.ToString(), MaxQueryValueLength)}")
                    .ToArray();
                currentActivity.SetTag("http.query", queryPairs);
            }
        }

        await Next(httpContext);
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value.Substring(0, maxLength);
}
