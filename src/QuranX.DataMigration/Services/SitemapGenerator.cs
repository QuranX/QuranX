using NLog;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;
using QuranX.Shared.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace QuranX.DataMigration.Services;

public interface ISitemapGenerator
{
    void Generate();
}

public sealed class SitemapGenerator : ISitemapGenerator
{
    private record class SitemapUrl(string Path, decimal? Priority);

    private const string BaseUrl = "https://quranx.com";
    private const string SitemapUrlPrefix = "/sitemaps/";

    private readonly ILogger Logger;
    private readonly IVerseRepository VerseRepository;
    private readonly IDictionaryRepository DictionaryRepository;
    private readonly IDictionaryEntryRepository DictionaryEntryRepository;
    private readonly ICommentatorRepository CommentatorRepository;
    private readonly ICommentaryRepository CommentaryRepository;
    private readonly IVerseAnalysisRepository VerseAnalysisRepository;
    private readonly IHadithRepository HadithRepository;
    private readonly IHadithCollectionRepository HadithCollectionRepository;
    private readonly ISettings Settings;

    public SitemapGenerator(
        ILogger logger,
        SitemapRepositories repositories,
        ISettings settings)
    {
        Logger = logger;
        VerseRepository = repositories.Verse;
        DictionaryRepository = repositories.Dictionary;
        DictionaryEntryRepository = repositories.DictionaryEntry;
        CommentatorRepository = repositories.Commentator;
        CommentaryRepository = repositories.Commentary;
        VerseAnalysisRepository = repositories.VerseAnalysis;
        HadithRepository = repositories.Hadith;
        HadithCollectionRepository = repositories.HadithCollection;
        Settings = settings;
    }

    public void Generate()
    {
        Logger.Debug("Generating site maps");
        string outputDir = GetOutputDirectory();
        Directory.Delete(outputDir, true);
        Directory.CreateDirectory(outputDir);

        string[] sitemapFileNames =
            Enumerable.Empty<string>()
            .Concat(GetHadithSitemapFileNames())
            .Concat([
                "verse-hadiths.xml",
                    "static-pages.xml",
                    "verses.xml",
                    "verse-tafsirs.xml",
                    "verse-analyses.xml",
                    "root-words.xml",
                    "dictionaries.xml",
                    "dictionary-entries.xml"
            ])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        WriteFile(
            Path.Combine(outputDir, "..", "sitemap.xml"),
            BuildSitemapIndexXml(sitemapFileNames.Select(f => SitemapUrlPrefix + f)));

        WriteFile(Path.Combine(outputDir, "static-pages.xml"), BuildUrlSitemapXml(GetStaticUrls()));
        WriteFile(Path.Combine(outputDir, "verses.xml"), BuildUrlSitemapXml(GetVerseUrls()));
        WriteFile(Path.Combine(outputDir, "verse-analyses.xml"), BuildUrlSitemapXml(GetVerseAnalysisUrls()));
        WriteFile(Path.Combine(outputDir, "root-words.xml"), BuildUrlSitemapXml(GetRootWordUrls()));
        WriteFile(Path.Combine(outputDir, "dictionaries.xml"), BuildUrlSitemapXml(GetDictionaryUrls()));
        WriteFile(Path.Combine(outputDir, "dictionary-entries.xml"), BuildUrlSitemapXml(GetDictionaryEntryUrls()));
        WriteFile(Path.Combine(outputDir, "verse-hadiths.xml"), BuildUrlSitemapXml(GetVerseHadithUrls()));

        IEnumerable<SitemapUrl> tafsirUrls =
            Enumerable.Empty<SitemapUrl>()
            .Concat(GetVerseTafsirUrls())
            .Concat(GetTafsirUrls());
        WriteFile(Path.Combine(outputDir, "verse-tafsirs.xml"), BuildUrlSitemapXml(tafsirUrls));

        foreach ((string FileName, IEnumerable<SitemapUrl> Urls) hadithSitemap in GetHadithSitemaps())
            WriteFile(Path.Combine(outputDir, hadithSitemap.FileName), BuildUrlSitemapXml(hadithSitemap.Urls));
    }

    private string GetOutputDirectory()
    {
        string outputDir = Path.Combine(Settings.DataPath, "..", "wwwroot", "sitemaps");
        return Path.GetFullPath(outputDir);
    }

    private void WriteFile(string filePath, string content)
        => File.WriteAllText(filePath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

    private static string BuildSitemapIndexXml(IEnumerable<string> sitemapPaths)
    {
        static string Escape(string value) => System.Security.SecurityElement.Escape(value);

        var sb = new StringBuilder(capacity: 16 * 1024);
        string now = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        foreach (string path in sitemapPaths.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            sb.AppendLine("  <sitemap>");
            sb.Append("    <loc>").Append(Escape(BaseUrl + path)).AppendLine("</loc>");
            sb.Append("    <lastmod>").Append(now).AppendLine("</lastmod>");
            sb.AppendLine("  </sitemap>");
        }

        sb.AppendLine("</sitemapindex>");
        return sb.ToString();
    }

    private static string BuildUrlSitemapXml(IEnumerable<SitemapUrl> urls)
    {
        static string Escape(string value) => System.Security.SecurityElement.Escape(value);

        var sb = new StringBuilder(capacity: 128 * 1024);
        string now = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var usedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        foreach (SitemapUrl url in urls)
        {
            if (!usedUrls.Add(url.Path))
                continue;

            sb.AppendLine("  <url>");
            sb.Append("    <loc>").Append(Escape(BaseUrl + url.Path)).AppendLine("</loc>");
            sb.Append("    <lastmod>").Append(now).AppendLine("</lastmod>");
            sb.AppendLine("    <changefreq>monthly</changefreq>");

            if (url.Priority.HasValue)
            {
                sb.Append("    <priority>")
                    .Append(url.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture))
                    .AppendLine("</priority>");
            }

            sb.AppendLine("  </url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    private IEnumerable<string> GetHadithSitemapFileNames()
        => GetHadithSitemaps().Select(s => s.FileName);

    private IEnumerable<(string FileName, IEnumerable<SitemapUrl> Urls)> GetHadithSitemaps()
    {
        IEnumerable<HadithCollection> collections = HadithCollectionRepository.GetAll();

        foreach (HadithCollection collection in collections)
        {
            IEnumerable<string> referenceCodes =
                HadithRepository
                    .GetAllReferences(collection.Code)
                    .Select(r => r.ReferenceCode)
                    .Where(code => !string.IsNullOrWhiteSpace(code))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(code => code, StringComparer.OrdinalIgnoreCase);

            foreach (string referenceCode in referenceCodes)
            {
                Logger.Debug($"Generating sitemap for Hadiths {collection.Code}/{referenceCode}");
                IEnumerable<SitemapUrl> urls =
                    HadithRepository
                        .GetAllReferences(collection.Code)
                        .Where(r => string.Equals(r.ReferenceCode, referenceCode, StringComparison.OrdinalIgnoreCase))
                        .Select(reference =>
                        {
                            HadithReferenceDefinition referenceDefinition = collection.GetReferenceDefinition(reference.ReferenceCode);
                            string referencePath = reference.GetPath(referenceDefinition);
                            return new SitemapUrl($"/Hadith/{referencePath}", Priority: 0.4m);
                        });

                yield return ($"hadiths_{collection.Code}_{referenceCode}.xml", urls);
            }
        }
    }

    private IEnumerable<SitemapUrl> GetStaticUrls()
    {
        Logger.Debug("Generating sitemap for static pages");
        yield return new SitemapUrl("/", Priority: 1.0m);
        yield return new SitemapUrl("/About", Priority: 0.1m);
        yield return new SitemapUrl("/Help", Priority: 0.1m);
        yield return new SitemapUrl("/Hadiths", Priority: 0.2m);
        yield return new SitemapUrl("/Tafsirs", Priority: 0.2m);
        yield return new SitemapUrl("/Dictionaries", Priority: 0.2m);
    }

    private IEnumerable<SitemapUrl> GetVerseUrls()
    {
        Logger.Debug("Generating sitemap for verses");
        foreach (VerseReference reference in VerseRepository.GetVerseReferences())
        {
            string verse = $"{reference.Chapter}.{reference.Verse}";
            yield return new SitemapUrl($"/{verse}", Priority: 0.8m);
        }
    }

    private IEnumerable<SitemapUrl> GetVerseAnalysisUrls()
    {
        Logger.Debug("Generating sitemap for verse analyses");
        foreach (VerseReference reference in VerseRepository.GetVerseReferences())
        {
            string verse = $"{reference.Chapter}.{reference.Verse}";
            yield return new SitemapUrl($"/Analysis/{verse}", Priority: 0.5m);
        }
    }

    private IEnumerable<SitemapUrl> GetDictionaryUrls()
    {
        Logger.Debug("Generating sitemap for dictionaries");
        IEnumerable<Dictionary> dictionaries = DictionaryRepository.GetAll();
        foreach (Dictionary dictionary in dictionaries)
            yield return new SitemapUrl($"/Dictionaries/{dictionary.Code}", Priority: 0.3m);
    }

    private IEnumerable<SitemapUrl> GetDictionaryEntryUrls()
    {
        Logger.Debug("Generating sitemap for dictionary entries");
        foreach (string root in DictionaryEntryRepository.GetAll())
            yield return new SitemapUrl($"/Dictionaries/{root}", Priority: 0.3m);
    }

    private IEnumerable<SitemapUrl> GetRootWordUrls()
    {
        Logger.Debug("Generating sitemap for root words");
        foreach (string root in DictionaryEntryRepository.GetAll())
        {
            if (VerseAnalysisRepository.GetForRoot(root).Any())
            {
                string letterNames = ArabicHelper.ArabicToLetterNames(root);
                yield return new SitemapUrl($"/Analysis/Root/{letterNames}", Priority: 0.5m);
            }
        }
    }

    private IEnumerable<SitemapUrl> GetTafsirUrls()
    {
        IEnumerable<Commentator> commentators = CommentatorRepository.GetAll();
        foreach (Commentator commentator in commentators)
        {
            Logger.Debug($"Generating sitemap for Tafsir {commentator.Code}");
            IEnumerable<VerseRangeReference> ranges = CommentaryRepository.GetVerseRangeReferences(commentator.Code);
            foreach (VerseRangeReference range in ranges)
                yield return new SitemapUrl($"/Tafsir/{commentator.Code}/{range.Chapter}.{range.FirstVerse}", Priority: 0.4m);
        }
    }

    private IEnumerable<SitemapUrl> GetVerseTafsirUrls()
    {
        int lastChapter = 0;
        foreach (VerseReference reference in VerseRepository.GetVerseReferences())
        {
            if (lastChapter != reference.Chapter)
            {
                lastChapter = reference.Chapter;
                Logger.Debug($"Generating sitemap for Tafsirs chapter {reference.Chapter}");
            }
            string verse = $"{reference.Chapter}.{reference.Verse}";
            if (CommentaryRepository.GetForVerse(reference.Chapter, reference.Verse).Any())
                yield return new SitemapUrl($"/Tafsirs/{verse}", Priority: 0.75m);
        }
    }

    private IEnumerable<SitemapUrl> GetVerseHadithUrls()
    {
        int lastChapter = 0;
        foreach (VerseReference reference in VerseRepository.GetVerseReferences())
        {
            if (reference.Chapter != lastChapter)
            {
                lastChapter = reference.Chapter;
                Logger.Debug($"Generating sitemap Hadiths for chapter {reference.Chapter}");
            }
            if (HadithRepository.GetForVerse(new VerseReference(reference.Chapter, reference.Verse)).Any())
            {
                string verse = $"{reference.Chapter}.{reference.Verse}";
                yield return new SitemapUrl($"/Hadiths/{verse}", Priority: 0.75m);
            }
        }
    }
}
