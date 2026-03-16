using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Api.V1.Endpoints;

public class GetVerses
{
    private readonly IChapterRepository ChapterRepository;
    private readonly IVerseRepository VerseRepository;

    public GetVerses(IChapterRepository chapterRepository, IVerseRepository verseRepository)
    {
        ChapterRepository = chapterRepository;
        VerseRepository = verseRepository;
    }

    public IEnumerable<ChapterAndVerseSelection> Execute(IEnumerable<VerseRangeReference> verseRangeReferences)
    {
        IEnumerable<Verse> retrievedVerses = VerseRepository.GetVerses(verseRangeReferences)
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
}
