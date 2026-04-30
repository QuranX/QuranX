#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModelContextProtocol.Server;
using QuranX.Shared;

namespace QuranX.Web.Middlewares;

public class McpTracingMiddleware
{
    private static readonly Lazy<Dictionary<string, ParameterInfo[]>> ToolParameters =
        new(BuildToolParameterMap);

    private static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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
        try
        {
            using JsonDocument document = await JsonDocument.ParseAsync(
                context.Request.Body,
                cancellationToken: context.RequestAborted);
            TryTagFromDocument(activity, document.RootElement);
        }
        catch (JsonException) { }
        finally
        {
            if (context.Request.Body.CanSeek)
                context.Request.Body.Position = 0;
        }

        await Next(context);
    }

    private static void TryTagFromDocument(Activity activity, JsonElement root)
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
            return;
        }

        string? toolName = nameElement.GetString();
        if (toolName is null)
            return;

        activity.SetTag("mcp.tool.name", toolName);

        if (!paramsElement.TryGetProperty("arguments", out JsonElement argsElement)
            || argsElement.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        ToolParameters.Value.TryGetValue(toolName, out ParameterInfo[]? parameters);

        foreach (JsonProperty property in argsElement.EnumerateObject())
        {
            ParameterInfo? param = parameters?.FirstOrDefault(p =>
                string.Equals(p.Name, property.Name, StringComparison.OrdinalIgnoreCase));
            TagArgument(activity, property.Name, property.Value, param?.ParameterType);
        }
    }

    private static void TagArgument(
        Activity activity,
        string name,
        JsonElement value,
        Type? paramType)
    {
        string tagKey = $"mcp.tool.arg.{name}";

        if (paramType is not null
            && TryFormatWithDisplayText(value, paramType, out object? formatted))
        {
            activity.SetTag(tagKey, formatted);
            return;
        }

        string fallback = value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Null => string.Empty,
            _ => value.GetRawText()
        };
        activity.SetTag(tagKey, fallback);
    }

    private static bool TryFormatWithDisplayText(
        JsonElement value,
        Type paramType,
        out object? formatted)
    {
        formatted = null;

        if (typeof(IGetDisplayText).IsAssignableFrom(paramType))
        {
            if (TryDeserialize(value, paramType, out object? instance)
                && instance is IGetDisplayText displayable)
            {
                formatted = displayable.GetDisplayText();
                return true;
            }
        }

        Type? elementType = GetCollectionElementType(paramType);
        if (elementType is not null
            && typeof(IGetDisplayText).IsAssignableFrom(elementType))
        {
            if (TryDeserialize(value, paramType, out object? instance)
                && instance is IEnumerable enumerable)
            {
                formatted = enumerable
                    .Cast<IGetDisplayText>()
                    .Select(x => x.GetDisplayText())
                    .ToArray();
                return true;
            }
        }

        return false;
    }

    private static bool TryDeserialize(JsonElement value, Type type, out object? result)
    {
        try
        {
            result = JsonSerializer.Deserialize(value, type, DeserializeOptions);
            return result is not null;
        }
        catch (JsonException)
        {
            result = null;
            return false;
        }
        catch (NotSupportedException)
        {
            result = null;
            return false;
        }
    }

    private static Type? GetCollectionElementType(Type type)
    {
        if (type == typeof(string))
            return null;
        if (type.IsArray)
            return type.GetElementType();
        foreach (Type iface in type.GetInterfaces())
        {
            if (iface.IsGenericType
                && iface.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return iface.GetGenericArguments()[0];
            }
        }
        return null;
    }

    private static Dictionary<string, ParameterInfo[]> BuildToolParameterMap()
    {
        Dictionary<string, ParameterInfo[]> map = new();
        Assembly assembly = typeof(McpTracingMiddleware).Assembly;
        const BindingFlags flags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

        foreach (Type type in assembly.GetTypes())
        {
            foreach (MethodInfo method in type.GetMethods(flags))
            {
                McpServerToolAttribute? attr =
                    method.GetCustomAttribute<McpServerToolAttribute>();
                if (attr is null)
                    continue;
                string toolName = string.IsNullOrEmpty(attr.Name)
                    ? method.Name
                    : attr.Name;
                map[toolName] = method.GetParameters();
            }
        }
        return map;
    }
}
