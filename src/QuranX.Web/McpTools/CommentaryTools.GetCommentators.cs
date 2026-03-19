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
        Title = "Get list of Quran commentators",
        UseStructuredContent = true,
        ReadOnly = true,
        Idempotent = true,
        Destructive = false,
        OpenWorld = false)]
    [Description("Gets a list of available commentators (mufassir)")]
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

