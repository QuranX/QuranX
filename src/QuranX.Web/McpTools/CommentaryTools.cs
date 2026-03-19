#nullable enable
using ModelContextProtocol.Server;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.McpTools;

[McpServerToolType]
public sealed partial class CommentaryTools
{
    private readonly ICommentatorRepository CommentatorRepository;
    private readonly ICommentaryRepository CommentaryRepository;

    public CommentaryTools(
        ICommentatorRepository commentatorRepository,
        ICommentaryRepository commentaryRepository)
    {
        CommentatorRepository = commentatorRepository;
        CommentaryRepository = commentaryRepository;
    }
}