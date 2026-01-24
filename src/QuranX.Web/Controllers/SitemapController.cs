using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;
using QuranX.Shared.Models;

namespace QuranX.Web.Controllers
{
	[Route("")]
	public class SitemapController : Controller
	{
		private readonly IVerseRepository VerseRepository;
		private readonly IDictionaryRepository DictionaryRepository;
		private readonly IDictionaryEntryRepository DictionaryEntryRepository;
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly ICommentaryRepository CommentaryRepository;
		private readonly IVerseAnalysisRepository VerseAnalysisRepository;
		private readonly IHadithRepository HadithRepository;
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public SitemapController(
			IVerseRepository verseRepository,
			IDictionaryRepository dictionaryRepository,
			IDictionaryEntryRepository dictionaryEntryRepository,
			ICommentatorRepository commentatorRepository,
			ICommentaryRepository commentaryRepository,
			IVerseAnalysisRepository verseAnalysisRepository,
			IHadithRepository hadithRepository,
			IHadithCollectionRepository hadithCollectionRepository)
		{
			VerseRepository = verseRepository;
			DictionaryRepository = dictionaryRepository;
			DictionaryEntryRepository = dictionaryEntryRepository;
			CommentatorRepository = commentatorRepository;
			CommentaryRepository = commentaryRepository;
			VerseAnalysisRepository = verseAnalysisRepository;
			HadithRepository = hadithRepository;
			HadithCollectionRepository = hadithCollectionRepository;
		}

		// =========================================================
		// Sitemap Index (sitemap.xml)
		// =========================================================

		[HttpGet("Sitemap.xml")]
		public async Task Index(CancellationToken cancellationToken)
		{
			try
			{
				string baseUrl = $"{Request.Scheme}://{Request.Host}";

				IEnumerable<string> sitemapPaths =
					Enumerable.Empty<string>()
					.Concat(GetHadithSitemapPaths())
					.Concat(new[]
					{
						"/Sitemap_VerseHadiths.xml",
						"/Sitemap_StaticPages.xml",
						"/Sitemap_Verses.xml",
						"/Sitemap_VerseTafsirs.xml",
						"/Sitemap_VerseAnalyses.xml",
						"/Sitemap_RootWords.xml",
						"/Sitemap_Dictionaries.xml",
						"/Sitemap_DictionaryEntries.xml"
					});

				Response.ContentType = "application/xml; charset=utf-8";
				await WriteSitemapIndexXmlAsync(baseUrl, sitemapPaths, Response.Body, cancellationToken);
				return;
			}
			catch (OperationCanceledException)
			{
				Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
				return;
			}
		}

		private IEnumerable<string> GetHadithSitemapPaths()
		{
			IEnumerable<HadithCollection> collections = HadithCollectionRepository.GetAll();

			foreach (HadithCollection collection in collections)
			{
				// "Reference" dimension: HadithReference.ReferenceCode
				IEnumerable<string> referenceCodes =
					HadithRepository
						.GetAllReferences(collection.Code)
						.Select(r => r.ReferenceCode)
						.Where(code => !string.IsNullOrWhiteSpace(code))
						.Distinct(StringComparer.OrdinalIgnoreCase)
						.OrderBy(code => code, StringComparer.OrdinalIgnoreCase);

				foreach (string referenceCode in referenceCodes)
					yield return $"/Sitemap_Hadiths_{collection.Code}_{referenceCode}.xml";
			}
		}

		private static async Task WriteSitemapIndexXmlAsync(string baseUrl, IEnumerable<string> sitemapPaths, Stream output, CancellationToken cancellationToken)
		{
			static string Escape(string value) => System.Security.SecurityElement.Escape(value);

			var settings = new System.Xml.XmlWriterSettings {
				Async = true,
				Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
				OmitXmlDeclaration = false,
				CloseOutput = false
			};

			using var writer = System.Xml.XmlWriter.Create(output, settings);
			await writer.WriteStartDocumentAsync();
			await writer.WriteStartElementAsync(prefix: null, localName: "sitemapindex", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");

			string now = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

			foreach (string path in sitemapPaths.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				cancellationToken.ThrowIfCancellationRequested();

				await writer.WriteStartElementAsync(null, "sitemap", null);

				await writer.WriteStartElementAsync(null, "loc", null);
				await writer.WriteStringAsync(Escape(baseUrl + path));
				await writer.WriteEndElementAsync();

				await writer.WriteStartElementAsync(null, "lastmod", null);
				await writer.WriteStringAsync(now);
				await writer.WriteEndElementAsync();

				await writer.WriteEndElementAsync();
			}

			await writer.WriteEndElementAsync();
			await writer.WriteEndDocumentAsync();

			await writer.FlushAsync();
			await output.FlushAsync(cancellationToken);
		}

		// =========================================================
		// Child sitemaps
		// =========================================================

		[HttpGet("Sitemap_StaticPages.xml")]
		public async Task StaticPages(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetStaticUrls(), cancellationToken);

		[HttpGet("Sitemap_Verses.xml")]
		public async Task Verses(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetVerseUrls(), cancellationToken);

		[HttpGet("Sitemap_VerseAnalyses.xml")]
		public async Task VerseAnalyses(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetVerseAnalysisUrls(), cancellationToken);

		[HttpGet("Sitemap_RootWords.xml")]
		public async Task RootWords(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetRootWordUrls(), cancellationToken);

		[HttpGet("Sitemap_Dictionaries.xml")]
		public async Task Dictionaries(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetDictionaryUrls(), cancellationToken);

		[HttpGet("Sitemap_DictionaryEntries.xml")]
		public async Task DictionaryEntries(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetDictionaryEntryUrls(), cancellationToken);

		[HttpGet("Sitemap_VerseHadiths.xml")]
		public async Task VerseHadiths(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(GetVerseHadithUrls(), cancellationToken);

		// Per your request: both GetVerseTafsirUrls() and GetTafsirUrls() go into Sitemap_VerseTafsirs.xml
		[HttpGet("Sitemap_VerseTafsirs.xml")]
		public async Task VerseTafsirs(CancellationToken cancellationToken)
			=> await WriteUrlSitemapAsync(
				Enumerable.Empty<SitemapUrl>()
					.Concat(GetVerseTafsirUrls())
					.Concat(GetTafsirUrls()),
				cancellationToken);

		// Hadith split: Sitemap_Hadiths_{Collection.Code}_{ReferenceCode}.xml
		[HttpGet("Sitemap_Hadiths_{collectionCode}_{referenceCode}.xml")]
		public async Task Hadiths(string collectionCode, string referenceCode, CancellationToken cancellationToken)
		{
			try
			{
				string baseUrl = $"{Request.Scheme}://{Request.Host}";

				IEnumerable<HadithCollection> collections = HadithCollectionRepository.GetAll();
				HadithCollection collection =
					collections.FirstOrDefault(c => string.Equals(c.Code, collectionCode, StringComparison.OrdinalIgnoreCase));

				if (collection == null)
				{
					Response.StatusCode = StatusCodes.Status404NotFound;
					return;
				}

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

				Response.ContentType = "application/xml; charset=utf-8";
				await WriteSitemapXmlAsync(baseUrl, urls, Response.Body, cancellationToken);
				return;
			}
			catch (OperationCanceledException)
			{
				Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
				return;
			}
		}

		private async Task WriteUrlSitemapAsync(IEnumerable<SitemapUrl> urls, CancellationToken cancellationToken)
		{
			try
			{
				string baseUrl = $"{Request.Scheme}://{Request.Host}";
				Response.ContentType = "application/xml; charset=utf-8";
				await WriteSitemapXmlAsync(baseUrl, urls, Response.Body, cancellationToken);
				return;
			}
			catch (OperationCanceledException)
			{
				Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
				return;
			}
		}

		// =========================================================
		// Writers
		// =========================================================

		private static async Task WriteSitemapXmlAsync(string baseUrl, IEnumerable<SitemapUrl> urls, Stream output, CancellationToken cancellationToken)
		{
			static string Escape(string value) => System.Security.SecurityElement.Escape(value);

			var settings = new System.Xml.XmlWriterSettings {
				Async = true,
				Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
				OmitXmlDeclaration = false,
				CloseOutput = false
			};

			using var writer = System.Xml.XmlWriter.Create(output, settings);
			await writer.WriteStartDocumentAsync();
			await writer.WriteStartElementAsync(prefix: null, localName: "urlset", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");

			string now = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			var usedUrls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

			foreach (SitemapUrl url in urls)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (!usedUrls.Add(url.Path))
					continue;

				await writer.WriteStartElementAsync(null, "url", null);

				await writer.WriteStartElementAsync(null, "loc", null);
				await writer.WriteStringAsync(Escape(baseUrl + url.Path));
				await writer.WriteEndElementAsync();

				await writer.WriteStartElementAsync(null, "lastmod", null);
				await writer.WriteStringAsync(now);
				await writer.WriteEndElementAsync();

				await writer.WriteStartElementAsync(null, "changefreq", null);
				await writer.WriteStringAsync("monthly");
				await writer.WriteEndElementAsync();

				if (url.Priority.HasValue)
				{
					await writer.WriteStartElementAsync(null, "priority", null);
					await writer.WriteStringAsync(url.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture));
					await writer.WriteEndElementAsync();
				}

				await writer.WriteEndElementAsync();

				if (usedUrls.Count % 500 == 0)
				{
					await writer.FlushAsync();
					await output.FlushAsync(cancellationToken);
				}
			}

			await writer.WriteEndElementAsync();
			await writer.WriteEndDocumentAsync();

			await writer.FlushAsync();
			await output.FlushAsync(cancellationToken);
		}

		// =========================================================
		// URL generators
		// =========================================================

		private static IEnumerable<SitemapUrl> GetStaticUrls()
		{
			yield return new SitemapUrl("/", Priority: 1.0m);
			yield return new SitemapUrl("/About", Priority: 0.1m);
			yield return new SitemapUrl("/Help", Priority: 0.1m);
			yield return new SitemapUrl("/Hadiths", Priority: 0.2m);
			yield return new SitemapUrl("/Tafsirs", Priority: 0.2m);
			yield return new SitemapUrl("/Dictionaries", Priority: 0.2m);
		}

		private IEnumerable<SitemapUrl> GetVerseUrls()
		{
			foreach (VerseReference reference in VerseRepository.GetVerseReferences())
			{
				string verse = $"{reference.Chapter}.{reference.Verse}";
				yield return new SitemapUrl($"/{verse}", Priority: 0.8m);
			}
		}

		private IEnumerable<SitemapUrl> GetVerseAnalysisUrls()
		{
			foreach (VerseReference reference in VerseRepository.GetVerseReferences())
			{
				string verse = $"{reference.Chapter}.{reference.Verse}";
				yield return new SitemapUrl($"/Analysis/{verse}", Priority: 0.5m);
			}
		}

		private IEnumerable<SitemapUrl> GetDictionaryUrls()
		{
			IEnumerable<Dictionary> dictionaries = DictionaryRepository.GetAll();
			foreach (Dictionary dictionary in dictionaries)
				yield return new SitemapUrl($"/Dictionaries/{dictionary.Code}", Priority: 0.3m);
		}

		private IEnumerable<SitemapUrl> GetDictionaryEntryUrls()
		{
			foreach (string root in DictionaryEntryRepository.GetAll())
				yield return new SitemapUrl($"/Dictionaries/{root}", Priority: 0.3m);
		}

		private IEnumerable<SitemapUrl> GetRootWordUrls()
		{
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
				IEnumerable<VerseRangeReference> ranges = CommentaryRepository.GetVerseRangeReferences(commentator.Code);
				foreach (VerseRangeReference range in ranges)
					yield return new SitemapUrl($"/Tafsir/{commentator.Code}/{range.Chapter}.{range.FirstVerse}", Priority: 0.4m);
			}
		}

		private IEnumerable<SitemapUrl> GetVerseTafsirUrls()
		{
			foreach (VerseReference reference in VerseRepository.GetVerseReferences())
			{
				string verse = $"{reference.Chapter}.{reference.Verse}";
				if (CommentaryRepository.GetForVerse(reference.Chapter, reference.Verse).Any())
					yield return new SitemapUrl($"/Tafsirs/{verse}", Priority: 0.75m);
			}
		}

		private IEnumerable<SitemapUrl> GetVerseHadithUrls()
		{
			foreach (VerseReference reference in VerseRepository.GetVerseReferences())
			{
				if (HadithRepository.GetForVerse(new VerseReference(reference.Chapter, reference.Verse)).Any())
				{
					string verse = $"{reference.Chapter}.{reference.Verse}";
					yield return new SitemapUrl($"/Hadiths/{verse}", Priority: 0.75m);
				}
			}
		}

		private sealed record SitemapUrl(string Path, decimal? Priority = null);
	}
}
