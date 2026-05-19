namespace QuranX.DocumentModel;

public record class WordPartLocation(
    int ChapterIndex,
    int VerseIndex,
    int WordIndex,
    int WordPartIndex);
