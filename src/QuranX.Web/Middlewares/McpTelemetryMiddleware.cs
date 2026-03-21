using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuranX.Web.Middlewares;

public class McpTelemetryMiddleware
{
    private static readonly ActivitySource McpActivitySource = new("QuranX.Mcp");
    private readonly RequestDelegate Next;

    public McpTelemetryMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.Request.Path.StartsWithSegments("/mcp"))
        {
            await Next(httpContext);
            return;
        }

        string jsonRpcMethod = null;
        string toolName = null;
        string body = null;
        var toolArgs = new System.Collections.Generic.Dictionary<string, string>();

        if (httpContext.Request.Method == "POST"
            && httpContext.Request.ContentType?.Contains("json") == true)
        {
            httpContext.Request.EnableBuffering();

            using var reader = new StreamReader(
                httpContext.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            body = await reader.ReadToEndAsync();
            httpContext.Request.Body.Position = 0;

            try
            {
                using JsonDocument doc = JsonDocument.Parse(body);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("method", out JsonElement methodElement))
                    jsonRpcMethod = methodElement.GetString();

                if (root.TryGetProperty("params", out JsonElement paramsElement))
                {
                    if (paramsElement.TryGetProperty("name", out JsonElement nameElement))
                        toolName = nameElement.GetString();

                    if (paramsElement.TryGetProperty("arguments", out JsonElement argsElement)
                        && argsElement.ValueKind == JsonValueKind.Object)
                    {
                        FlattenJsonObject(argsElement, "mcp.tool.arg", toolArgs);
                    }
                }
            }
            catch (JsonException)
            {
                // Not valid JSON, skip parsing
            }
        }

        using Activity activity =
            McpActivitySource
            .StartActivity(toolName is not null ? $"mcp.tool.{toolName}" : "mcp.request");

        if (activity is not null)
        {
            activity.SetTag("http.form", body);
            activity.SetTag("http.method", httpContext.Request.Method);
            activity.SetTag("http.path", httpContext.Request.Path.Value);
            activity.SetTag("http.request.content_type", httpContext.Request.ContentType ?? "null");

            if (jsonRpcMethod is not null)
                activity.SetTag("mcp.jsonrpc.method", jsonRpcMethod);

            if (toolName is not null)
                activity.SetTag("mcp.tool.name", toolName);

            foreach (var arg in toolArgs)
                activity.SetTag($"mcp.tool.arg.{arg.Key}", arg.Value);
        }

        try
        {
            await Next(httpContext);
        }
        catch (Exception ex)
        {
            if (activity is not null)
                activity.AddException(ex);
            throw;
        }
    }

    private static void FlattenJsonObject(
        JsonElement element,
        string prefix,
        System.Collections.Generic.Dictionary<string, string> tags)
    {
        foreach (JsonProperty prop in element.EnumerateObject())
        {
            string key = $"{prefix}.{prop.Name}";
            switch (prop.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    FlattenJsonObject(prop.Value, key, tags);
                    break;
                case JsonValueKind.Array:
                    tags[key] = prop.Value.GetRawText();
                    break;
                default:
                    tags[key] = prop.Value.ToString();
                    break;
            }
        }
    }
}
