using QuranX.Shared.Models;
using System.Collections.Generic;

namespace QuranX.Web.Models;

public class CommentariesForVerse
{
    public readonly ChapterData Chapter;
    public readonly int VerseNumber;
    public readonly IEnumerable<CommentatorAndCommentary> Commentaries;
    public readonly SelectChapterAndVerse SelectChapterAndVerse;

    public CommentariesForVerse(
        ChapterData chapter,
        int verseNumber,
        IEnumerable<CommentatorAndCommentary> commentaries,
        SelectChapterAndVerse selectChapterAndVerse)
    {
        Chapter = chapter;
        VerseNumber = verseNumber;
        Commentaries = commentaries;
        SelectChapterAndVerse = selectChapterAndVerse;
    }
}