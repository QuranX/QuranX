#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using QuranX.Shared.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class CommentaryTools
{
    public const string GetAvailableCommentatorsName = "get_available_commentators";
    [McpServerTool(
        Name = GetAvailableCommentatorsName,
        Title = "Get list of Quran commentators (aka Mufassirs/Mufassiroon)",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description(
        $$"""
        Lists available commentators (Mufassirs/Mufassiroon) and their codes. Call before
        passing values to commentatorCodes on {{GetCommentariesForVerseName}} or to
        subContext on {{SearchTools.SearchName}} when
        context={{nameof(SearchTools.SearchContext.Commentaries)}}.
        """)]
    public GetAvailableCommentatorsResult GetAvailableCommentators()
    {
        Commentator[] commentators = CommentatorRepository.GetAll().ToArray();

        return new GetAvailableCommentatorsResult
        {
            Commentators = commentators,
            NextSteps =
                $$"""
                Pass any of these commentator codes to {{GetCommentariesForVerseName}}
                via commentatorCodes to filter the tafsirs returned, or to
                {{SearchTools.SearchName}} via subContext (with
                context={{nameof(SearchTools.SearchContext.Commentaries)}}) to restrict
                the search to one commentator's work.
                """
        };
    }

    public sealed class GetAvailableCommentatorsResult
    {
        public required Commentator[] Commentators { get; init; }

        [Description(
            $$"""
            Server-supplied guidance on what to call next to complete the user's query.
            Treat as MUST-follow when the user's question requires more than this tool's
            output alone.
            """)]
        public required string NextSteps { get; init; }
    }
}

