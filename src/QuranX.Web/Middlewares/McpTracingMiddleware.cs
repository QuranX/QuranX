#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace QuranX.Web.Middlewares;

public class McpTracingMiddleware
{
    private readonly RequestDelegate Next;

    public McpTracingMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (Activity.Current is not Activity activity
            || !HttpMethods.IsPost(context.Request.Method)
            || context.Request.ContentType is not string contentType
            || !contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            await Next(context);
            return;
        }

        context.Request.EnableBuffering();
        (string? toolName, Dictionary<string, string>? arguments) =
            await TryReadToolCall(context);

        if (toolName is not null)
        {
            activity.SetTag("mcp.tool.name", toolName);
            if (arguments is not null)
            {
                foreach ((string key, string value) in arguments)
                    activity.SetTag($"mcp.tool.arg.{key}", value);
            }
        }

        await Next(context);
    }

    private static async Task<(string? ToolName, Dictionary<string, string>? Arguments)>
        TryReadToolCall(HttpContext context)
    {
        try
        {
            using JsonDocument document = await JsonDocument.ParseAsync(
                context.Request.Body,
                cancellationToken: context.RequestAborted);
            return ExtractToolCall(document.RootElement);
        }
        catch (JsonException)
        {
            return (null, null);
        }
        finally
        {
            if (context.Request.Body.CanSeek)
                context.Request.Body.Position = 0;
        }
    }

    private static (string? ToolName, Dictionary<string, string>? Arguments) ExtractToolCall(
        JsonElement root)
    {
        JsonElement target = root.ValueKind switch
        {
            JsonValueKind.Object => root,
            JsonValueKind.Array when root.GetArrayLength() > 0 => root[0],
            _ => default
        };

        if (target.ValueKind != JsonValueKind.Object
            || !target.TryGetProperty("method", out JsonElement methodElement)
            || methodElement.ValueKind != JsonValueKind.String
            || methodElement.GetString() != "tools/call"
            || !target.TryGetProperty("params", out JsonElement paramsElement)
            || paramsElement.ValueKind != JsonValueKind.Object
            || !paramsElement.TryGetProperty("name", out JsonElement nameElement)
            || nameElement.ValueKind != JsonValueKind.String)
        {
            return (null, null);
        }

        string? toolName = nameElement.GetString();
        if (toolName is null)
            return (null, null);

        Dictionary<string, string>? arguments = null;
        if (paramsElement.TryGetProperty("arguments", out JsonElement argsElement)
            && argsElement.ValueKind == JsonValueKind.Object)
        {
            arguments = new Dictionary<string, string>();
            foreach (JsonProperty property in argsElement.EnumerateObject())
            {
                arguments[property.Name] = property.Value.ValueKind switch
                {
                    JsonValueKind.String => property.Value.GetString() ?? string.Empty,
                    JsonValueKind.Null => string.Empty,
                    _ => property.Value.GetRawText()
                };
            }
        }

        return (toolName, arguments);
    }
}
