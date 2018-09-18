using QuranX.Models;
using QuranX.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuranX.Shared.Models;

namespace QuranX.Controllers
{
	public class QuranController : Controller
	{
		[OutputCache(Duration = Consts.CacheTimeInSeconds)]
		public ActionResult Chapters()
		{
			return View(SharedData.Document.QuranDocument.Chapters);
		}

		[HttpPost]
		public ActionResult RedirectToChapterVerse(int chapter, int verse)
		{
			return RedirectToAction("Verse", new
			{
				Chapter = chapter,
				Verse = verse,
				lastVerse = -1
			});
		}

		[OutputCache(Duration = Consts.CacheTimeInSeconds, VaryByCustom = "translations")]
		public ActionResult Verse(
			int chapter,
			int verse,
			int lastVerse,
			string fromSearch, //Legacy
			string allTranslations,
			int context = 0)
		{
			ViewBag.AllTranslations =
				string.Compare(allTranslations, "y", true) == 0
				|| string.Compare(fromSearch, "y", true) == 0;
			if (context != 0)
				ViewBag.AnchorPoint = chapter + "." + verse;

			var model = CreateVersesModel(null, chapter, verse, lastVerse, context);
			return View(model);
		}

		[HttpPost]
		public ActionResult Verse(
			string[] translations,
			int chapter,
			int verse,
			int lastVerse,
			int context = 0)
		{
			ResetTranslations(ref translations);
			var model = CreateVersesModel(translations, chapter, verse, lastVerse, context);
			return PartialView("VersesView", model.Verses);
		}

		[OutputCache(Duration = Consts.CacheTimeInSeconds, VaryByCustom = "translations")]
		public ActionResult MultipleVerses(string verses)
		{
			SetTranslationsViewBag();
			ViewBag.VerseReferences = verses;
			var references = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			var model = SharedData.Document.QuranDocument.GetVersesInRange(references);
			return View(model);
		}

		[HttpPost]
		public ActionResult MultipleVerses(string[] translations, string verses)
		{
			ResetTranslations(ref translations);
			var references = verses.Split(',')
				.ToList()
				.ConvertAll(x => VerseRangeReference.Parse(x));
			var model = SharedData.Document.QuranDocument.GetVersesInRange(references);
			return PartialView("VersesView", model);
		}

		void ResetTranslations(ref string[] translations)
		{
			var cookie = new HttpCookie("translations");
			if (translations == null)
				translations = new string[] { };
			cookie.Value = string.Join(",", translations);
			cookie.Expires = DateTime.UtcNow.AddYears(10);
			Response.Cookies.Add(cookie);
			SetTranslationsViewBag(translations);
		}

		Quran_Verses CreateVersesModel(
			string[] translations,
			int chapter,
			int verse,
			int lastVerse,
			int numberOfVersesBeforeAndAfter)
		{
			if (lastVerse == -1)
				lastVerse = verse;

			verse -= numberOfVersesBeforeAndAfter;
			if (verse < 1)
				verse = 1;
			lastVerse += numberOfVersesBeforeAndAfter;

			ViewBag.Chapter = chapter;
			ViewBag.Verse = verse;
			ViewBag.CanShowVerseRange = true;
			SetTranslationsViewBag(translations);
			var model = new Quran_Verses(
					chapter: chapter,
					verse: verse,
					lastVerse: lastVerse
				);
			return model;
		}

		void SetTranslationsViewBag(string[] translations = null)
		{
			if (translations == null)
			{
				if (Request.Cookies.AllKeys.Contains("translations"))
					translations = Request.Cookies["translations"].Value.Split(',');
				else
					translations = SharedData.Document.QuranDocument.GetDefaultTranslatorCodes();

			}
			ViewBag.Translations = new HashSet<string>(translations, StringComparer.InvariantCultureIgnoreCase);
		}



	}
}
