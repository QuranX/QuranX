using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Api.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public class CommentatorsEndpoint : IApiEndpoint
{
    public const string UrlPath = "/commentators";

    private ICommentatorRepository CommentatorRepository;

    public CommentatorsEndpoint(ICommentatorRepository commentatorRepository)
    {
        CommentatorRepository = commentatorRepository;
    }

    void IApiEndpoint.Register(RouteGroupBuilder builder)
    {
        builder
            .MapGet(
                UrlPath,
                () =>
                {
                    CommentatorsEndpointResult result = Execute();
                    return Results.Ok(result);
                }
            )
            .WithName("GetCommentators")
            .WithSummary("Get commentators list")
            .WithDescription("Returns the list of available commentators")
            .Produces<CommentatorsEndpointResult>();
    }

    public CommentatorsEndpointResult Execute()
    {
        IEnumerable<Commentator> commentators =
            CommentatorRepository
            .GetAll()
            .Select(x => new Commentator(code: x.Code, description: x.Description));
        return new CommentatorsEndpointResult(commentators);

    }

    public class CommentatorsEndpointResult
    {
        public IEnumerable<Commentator> Commentators { get; }

        public CommentatorsEndpointResult(IEnumerable<Commentator> commentators)
        {
            Commentators = commentators ?? throw new ArgumentNullException(nameof(commentators));
        }
    }


}
