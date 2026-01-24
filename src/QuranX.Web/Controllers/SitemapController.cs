using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;
using QuranX.Shared.Models;

namespace QuranX.Web.Controllers
{
	[Route("sitemap.xml")]
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

		[HttpGet]
		public async Task Index(CancellationToken cancellationToken)
		{
			try
			{
				string baseUrl = $"{Request.Scheme}://{Request.Host}";

				IEnumerable<SitemapUrl> urls =
					Enumerable.Empty<SitemapUrl>()
					.Concat(GetHadithUrls())
					.Concat(GetVerseHadithUrls())
					.Concat(GetStaticUrls())
					.Concat(GetVerseUrls())
					.Concat(GetVerseTafsirUrls())
					.Concat(GetVerseAnalysisUrls())
					.Concat(GetRootWordUrls())
					.Concat(GetDictionaryUrls())
					.Concat(GetDictionaryEntryUrls())
					.Concat(GetTafsirUrls());

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

		private static async Task WriteSitemapXmlAsync(string baseUrl, IEnumerable<SitemapUrl> urls, Stream output, CancellationToken cancellationToken)
		{
			static string Escape(string value) => System.Security.SecurityElement.Escape(value);

			var settings = new System.Xml.XmlWriterSettings
			{
				Async = true,
				Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
				OmitXmlDeclaration = false,
				CloseOutput = false
			};

			using var writer = System.Xml.XmlWriter.Create(output, settings);
			await writer.WriteStartDocumentAsync();
			await writer.WriteStartElementAsync(prefix: null, localName: "urlset", ns: "http://www.sitemaps.org/schemas/sitemap/0.9");

			string now = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			var usedUrls = new HashSet<string>();

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
			IEnumerable<Dictionary> dictionaries = DictionaryRepository.GetAll();

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

			foreach (VerseReference reference in VerseRepository.GetVerseReferences())
			{
				string verse = $"{reference.Chapter}.{reference.Verse}";
				if (CommentaryRepository.GetForVerse(reference.Chapter, reference.Verse).Any())
					yield return new SitemapUrl($"/Tafsirs/{verse}", Priority: 0.75m);
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

		private IEnumerable<SitemapUrl> GetHadithUrls()
		{
			IEnumerable<HadithCollection> collections = HadithCollectionRepository.GetAll();
			foreach (HadithCollection collection in collections)
			{
				IEnumerable<HadithReference> hadithReferences = HadithRepository.GetAllReferences(collection.Code);
				foreach (HadithReference reference in hadithReferences)
				{
					HadithReferenceDefinition referenceDefinition = collection.GetReferenceDefinition(reference.ReferenceCode);
					string referencePath = reference.GetPath(referenceDefinition);
					string url = $"/Hadith/{referencePath}";
					yield return new SitemapUrl(url, Priority: 0.4m);
				}
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


		private static string BuildSitemapXml(string baseUrl, IEnumerable<SitemapUrl> urls, CancellationToken cancellationToken)
		{
			static string Escape(string value) => System.Security.SecurityElement.Escape(value);

			var result = new StringBuilder();
			result.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
			result.Append("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

			string now = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			var usedUrls = new HashSet<string>();
			foreach (SitemapUrl url in urls)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (usedUrls.Contains(url.Path))
					continue;

				usedUrls.Add(url.Path);
				result.Append("<url>");
				result.Append("<loc>");
				result.Append(Escape(baseUrl + url.Path));
				result.Append("</loc>");
				result.Append("<lastmod>");
				result.Append(now);
				result.Append("</lastmod>");
				result.Append("<changefreq>");
				result.Append("monthly");
				result.Append("</changefreq>");
				if (url.Priority.HasValue)
				{
					result.Append("<priority>");
					result.Append(url.Priority.Value.ToString("0.0", CultureInfo.InvariantCulture));
					result.Append("</priority>");
				}
				result.Append("</url>");
			}
			result.Append("</urlset>");
			return result.ToString();
		}

		private sealed record SitemapUrl(string Path, decimal? Priority = null);
	}
}
