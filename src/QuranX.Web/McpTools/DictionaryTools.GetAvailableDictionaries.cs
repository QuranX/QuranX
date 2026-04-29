#nullable enable
using ModelContextProtocol.Server;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class DictionaryTools
{
    public const string GetAvailableDictionariesName = "get_available_dictionaries";

    [McpServerTool(
        Name = GetAvailableDictionariesName,
        Title = "Get available dictionaries",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Lists available Arabic dictionaries with their codes and copyright notices. Call
        before passing a value to dictionaryCode on {{GetDictionaryEntriesName}} to
        restrict to a specific dictionary.
        """)]
    public GetAvailableDictionariesResult GetAvailableDictionaries()
    {
        var dictionaries = DictionaryRepository
            .GetAll()
            .Select(d => new DictionaryInfo
            {
                Code = d.Code,
                Name = d.Name,
                Copyright = d.Copyright
            })
            .ToList();

        return new GetAvailableDictionariesResult
        {
            Dictionaries = dictionaries
        };
    }

    public sealed class GetAvailableDictionariesResult
    {
        public required IReadOnlyList<DictionaryInfo> Dictionaries { get; init; }
    }

    public sealed class DictionaryInfo
    {
        public required string Code { get; init; }
        public required string Name { get; init; }
        public required string Copyright { get; init; }
    }
}
