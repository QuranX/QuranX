#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Models;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.McpTools;

partial class CommentaryTools
{
    [McpServerTool(
        Name = "get_available_commentators",
        Title = "Get list of Quran commentators (aka Mufassirs/Mufassiroon)",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets a list of available commentators (aka Mufassirs/Mufassiroon). Use the returned codes with get_commentaries_for_quran_verses or as subContext in search.")]
    public GetAvailableCommentatorsResult GetAvailableCommentators()
    {
        Commentator[] commentators = CommentatorRepository.GetAll().ToArray();

        return new GetAvailableCommentatorsResult
        {
            Commentators = commentators
        };
    }

    public sealed class GetAvailableCommentatorsResult
    {
        public required Commentator[] Commentators { get; init; }
    }
}

