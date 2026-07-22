using Lucene.Net.Util;

namespace QuranX.Persistence.Services;

public static class Consts
{
    public const string FullTextFieldName = "_FullText";
    public static readonly LuceneVersion LuceneVersion = LuceneVersion.LUCENE_48;
    public const string SerializedObjectFieldName = "_Object";
    public const string SerializedObjectTypeFieldName = "_Type";

    // Upper bound on user-supplied search text. Anything longer is truncated before
    // parsing to avoid pathological queries; normal searches are far shorter.
    public const int MaxQueryLength = 1000;
}
