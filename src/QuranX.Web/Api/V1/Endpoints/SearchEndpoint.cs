#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi;
using QuranX.Persistence.Services;
using QuranX.Web.Services;
using QuranX.Web.Views.Search;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json.Serialization;

namespace QuranX.Web.Api.V1.Endpoints;

public class SearchEndpoint : IApiEndpoint
{
    public const string UrlPath = "/search";

    private readonly ISearchEngine SearchEngine;
    private readonly ISearchResultWithLinkFactory SearchResultWithLinkFactory;

    public SearchEndpoint(ISearchEngine searchEngine, ISearchResultWithLinkFactory searchResultWithLinkFactory)
    {
        SearchEngine = searchEngine;
        SearchResultWithLinkFactory = searchResultWithLinkFactory;
    }

    void IApiEndpoint.Register(RouteGroupBuilder builder)
    {
        builder
            .MapGet(
                UrlPath,
                (
                    [FromQuery(Name = "query")]
                    [Description("Search text or Lucene search-engine expression")]
                    string query,

                    [FromQuery(Name = "scope")]
                    SearchScope? searchScope,

                    [FromQuery(Name = "sub-scope")]
                    string? subScope
                ) =>
                {
                    SearchEndpointResult? result = Execute(query, searchScope, subScope);
                    if (result is null)
                        return Results.BadRequest();
                    return Results.Ok(result);
                }
            )
            .AddOpenApiOperationTransformer(async (operation, context, cancellationToken) =>
            {
                IOpenApiParameter? parameter = operation.Parameters?.SingleOrDefault(x => x.Name == "scope");
                if (parameter is null)
                    return;

                ApiParameterDescription? parameterDescription = context
                    .Description
                    .ParameterDescriptions
                    .SingleOrDefault(x => x.Name == "scope");

                OpenApiSchema parameterSchema = await
                    context.GetOrCreateSchemaAsync(
                        type: typeof(SearchScope),
                        parameterDescription: parameterDescription,
                        cancellationToken: cancellationToken
                    );

                ((OpenApiParameter)parameter).Schema = parameterSchema;

            })
            .WithName("GetSearch")
            .WithSummary("Search all data")
            .WithDescription("Searches all data")
            .Produces<SearchEndpointResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public SearchEndpointResult? Execute(string query, SearchScope? searchScope, string? subScope)
    {
        const string badRequest = "badrequest";

        if (string.IsNullOrWhiteSpace(query))
            return null;

        string? context = searchScope switch
        {
            null => null,
            SearchScope.All => null,
            SearchScope.Quran => SearchContexts.Quran,
            SearchScope.Commentaries => SearchContexts.Commentaries,
            SearchScope.Hadiths => SearchContexts.Hadiths,
            _ => badRequest
        };
        if (context == badRequest)
            return null;

        IEnumerable<SearchResultWithLink> searchResultsWithLink = null!;
        try
        {
            IEnumerable<Persistence.Models.SearchResult> searchResults =
                SearchEngine.Search(query, context, subScope, out int totalResults);
            searchResultsWithLink =
                searchResults.Select(SearchResultWithLinkFactory.Create);
            return new SearchEndpointResult(totalResults);
        }
        catch (Lucene.Net.QueryParsers.Classic.ParseException)
        {
            searchResultsWithLink = new List<SearchResultWithLink>();
            return null;
        }

    }

    [JsonConverter(typeof(JsonStringEnumConverter<SearchScope>))]
    public enum SearchScope
    {
        All,
        Quran,
        Commentaries,
        Hadiths
    }

    public class SearchEndpointResult
    {
        public int TotalResults { get; }

        public SearchEndpointResult(int totalResults)
        {
            TotalResults = totalResults;
        }
    }
}
