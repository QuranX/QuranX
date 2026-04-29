using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class HadithTools
{
    public const string GetAvailableHadithCollectionsName = "get_available_hadith_collections";

    [McpServerTool(
        Name = GetAvailableHadithCollectionsName,
        Title = "Get a list of available hadith collections and hadith reference definitions",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Lists hadith collections (e.g. Bukhari, Muslim, Tirmidhi) with their codes and
        reference scheme definitions. Call before passing values to hadithCollectionCodes
        on {{GetHadithsForVerseName}}, or to subContext on {{SearchTools.SearchName}} when
        context={{nameof(SearchTools.SearchContext.Hadiths)}}. The reference definitions
        describe what fields a {{nameof(HadithReference)}} of that collection carries.
        """)]
    public GetHadithCollectionsResult GetHadithCollections()
    {
        IEnumerable<HadithCollection> collections = HadithCollectionRepository.GetAll();
        return new GetHadithCollectionsResult { HadithCollections = collections.ToArray() };
    }

    public sealed class GetHadithCollectionsResult
    {
        public required HadithCollection[] HadithCollections { get; init; }
    }
}
