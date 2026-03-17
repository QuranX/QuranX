#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public class VersesEndpoint : IApiEndpoint
{
    public const string UrlPath = "/verses";

    private readonly IChapterRepository ChapterRepository;
    private readonly IVerseRepository VerseRepository;

    public VersesEndpoint(IChapterRepository chapterRepository, IVerseRepository verseRepository)
    {
        ChapterRepository = chapterRepository;
        VerseRepository = verseRepository;
    }


    void IApiEndpoint.Register(RouteGroupBuilder builder)
    {
        builder.MapGet(
        UrlPath,
        (
            [FromQuery(Name = "refs")]
            [Description("Comma-separated verse references such as 1.1,4.34,12.1-7")]
            string refs,

            [FromQuery(Name = "translations")]
            [Description($"Optional comma-separated translation codes (see {InterpretersEndpoint.UrlPath})")]
            string? translations,

            [FromServices]
            VersesEndpoint handler
        ) =>
        {
            VerseRangeReference[] verseReferences =
                refs
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(VerseRangeReference.Parse)
                .OfType<VerseRangeReference>()
                .ToArray();

            if (!verseReferences.Any())
                return Results.BadRequest();

            string[] parsedTranslations = string.IsNullOrWhiteSpace(translations)
                ? []
                : translations.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            IEnumerable<ChapterAndVerseSelection> response = handler.Execute(verseReferences, parsedTranslations);
            return Results.Ok(response);
        })
        .WithName("Verses")
        .WithSummary("Get one or more verses by reference")
        .WithDescription("Accepts comma-separated verse references and optional comma-separated translations")
        .Produces<VersesEndpointResult>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public IEnumerable<ChapterAndVerseSelection> Execute(
        IEnumerable<VerseRangeReference> verseRangeReferences,
        IEnumerable<string> translations)
    {
        IEnumerable<Verse> retrievedVerses = VerseRepository
            .GetVerses(verseRangeReferences, translations)
            .OrderBy(x => x.ChapterNumber)
            .ThenBy(x => x.VerseNumber);

        var result = new List<ChapterAndVerseSelection>();
        foreach (VerseRangeReference verseRangeReference in verseRangeReferences)
        {
            IEnumerable<Verse> currentSelection =
                retrievedVerses
                .Where(x => verseRangeReference.Includes(x.ChapterNumber, x.VerseNumber));
            var chapterAndSelection = new ChapterAndVerseSelection(ChapterRepository.Get(verseRangeReference.Chapter), currentSelection);
            result.Add(chapterAndSelection);
        }

        return result;
    }

    public class VersesEndpointResult
    {
        public IEnumerable<ChapterAndVerseSelection> ChaptersAndVerses { get; }

        public VersesEndpointResult(IEnumerable<ChapterAndVerseSelection> chaptersAndVerses)
        {
            ChaptersAndVerses = chaptersAndVerses ?? throw new ArgumentNullException(nameof(chaptersAndVerses));
        }
    }
}
