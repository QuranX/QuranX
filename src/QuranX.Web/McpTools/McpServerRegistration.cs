using ModelContextProtocol.Server;

namespace QuranX.Web.McpTools;

internal static class McpServerRegistration
{
    internal static void Register(McpServerOptions options)
    {
        options.ServerInfo = new ModelContextProtocol.Protocol.Implementation
        {
            Name = "QuranX",
            Title = "Qur'an, Tafsirs, and Hadiths",
            Version = "1.0.0",
            WebsiteUrl = "https://quranx.com",
            Description =
               $"""
                An encyclopaedia of Islam.
                Search and explore the Quran, classical commentaries (tafsirs), and hadith collections.
                Look up verses with multiple English translations, Arabic text, and transliteration.
                Find scholarly commentary from classical scholars on any verse.
                Search across major hadith collections by reference or keyword.
                """,
        };
        options.ServerInstructions =
            $$"""
            You are an academic research tool. You do not offer moral opinions, merely facts based only on the data provided by this service.
            Use get_available_commentators to discover valid Tafsir/Commentary codes and
              get_available_hadith_collections to discover valid Hadith collection codes
              before filtering searches or fetching content.
            Treat Tafsirs/Commentators/Commentaries as synonymous for Commentary.

            Common workflows:
            - Verses about a topic: call {{SearchTools.SearchName}} with context=Quran, then {{QuranTools.GetVersesName}} to fetch the verses' contents.
            - Commentaries of verses about a topic: call {{SearchTools.SearchName}} with context=Quran, then {{CommentaryTools.GetCommentariesForVerseName}} for each verse in {{nameof(SearchTools.SearchResult.VerseReferences)}} to fetch the commentaries' contents.
            - Hadiths linked to verses about a topic: call {{SearchTools.SearchName}} with context=Quran, then {{HadithTools.GetHadithsForVerseName}} for each verse to fetch the linked hadiths' contents.
            - Hadiths matching a keyword: call {{SearchTools.SearchName}} with context=Hadiths, then {{HadithTools.GetHadithsName}} to fetch the hadiths' contents.
            - Hadiths in a specific collection by keyword: call {{HadithTools.GetAvailableHadithCollectionsName}} for valid codes, then call {{SearchTools.SearchName}} with context=Hadiths and subContext=<collection code>, then call {{HadithTools.GetHadithsName}} to fetch the hadiths' contents.
            - Tafsirs from a specific commentator by keyword: call {{CommentaryTools.GetAvailableCommentatorsName}} for valid codes, then call {{SearchTools.SearchName}} with context=Commentaries and subContext=<commentator code>, then call {{CommentaryTools.GetCommentariesForVerseName}} to fetch the commentaries' contents for the verses in the search result.
            Note: search with context=WholeSite returns keyword matches across all corpora — it does NOT return hadiths linked to matched verses.
            """;
    }
}
