using QuranX.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace QuranX.Controllers
{
	[OutputCache(Duration = 0, NoStore = true)]
	public class SiteMapController : Controller
	{
		const string Domain = "http://QuranX.com";
		const int PageSize = 10 * 1000;
		readonly DateTime LastMod = new DateTime(2016, 02, 11, 18, 18, 00);

		public SiteMapResult Quran()
		{
			var urls = new List<string>();
			foreach (var chapter in SharedData.Document.QuranDocument.Chapters)
				foreach (var verse in chapter.Verses)
					urls.Add(string.Format("{0}/{1}.{2}", Domain, chapter.Index, verse.Index));
			return new SiteMapResult(urls, LastMod);
		}

		public SiteMapResult Tafsir(string tafsirCode, int pageIndex)
		{
			pageIndex--;
			var urls = new List<string>();
			var tafsir = SharedData.Document.TafsirDocument[tafsirCode];
			var comments = tafsir.Comments.OrderBy(x => x.VerseReference).Skip(pageIndex * PageSize).Take(PageSize);
			foreach (var comment in comments)
				urls.Add(string.Format("{0}/tafsir/{1}/{2}.{3}", Domain, tafsirCode, comment.VerseReference.Chapter, comment.VerseReference.FirstVerse));
			return new SiteMapResult(urls, LastMod);
		}

		public SiteMapResult Hadith(string collectionCode, int pageIndex)
		{
			pageIndex--;
			var urls = new List<string>();
			var hadithCollection = SharedData.Document.HadithDocument[collectionCode];
			var hadiths = hadithCollection.Hadiths.OrderBy(x => x.PrimaryReference).Skip(pageIndex * PageSize).Take(PageSize);
			foreach (var hadith in hadiths)
			{
				foreach (HadithReference hadithReference in hadith.References)
				{
					var referenceDefinition = hadithCollection.GetReferenceDefinition(hadithReference.Code);
					string url = Domain + "/hadith/" + collectionCode + "/" + hadithReference.Code + "/";
					for (int index = 0; index < hadithReference.Values.Length; index++)
						url += referenceDefinition.PartNames[index] + "-" + hadithReference[index] + "/";
					urls.Add(url);
				}
			}
			return new SiteMapResult(urls, LastMod);
		}
	}
}
