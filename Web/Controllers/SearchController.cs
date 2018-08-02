using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucene.Net.QueryParsers;
using QuranX.Models;

namespace QuranX.Controllers
{
	public class SearchController : Controller
	{
		public ActionResult Index(string q, string context)
		{
			ViewBag.HideChapterVerseQuickJump = true;
			CreateContextOptions(context);
			string type = GetContextType(context);
			string group = GetContextGroup(context);

			var additionalFiltering = new List<KeyValuePair<string, string>>();
			if (!string.IsNullOrEmpty(type))
				additionalFiltering.Add(new KeyValuePair<string, string>("Type", type));
			if (!string.IsNullOrEmpty(group))
				additionalFiltering.Add(new KeyValuePair<string, string>("Group", group));

			var model = ExecuteSearch(q, additionalFiltering);
			return View(model);
		}

		[OutputCache(Duration = Consts.CacheTimeInSeconds)]
		public ActionResult Help()
		{
			ViewBag.HideChapterVerseQuickJump = true;
			return View();
		}

		string GetContextType(string context)
		{
			if (string.IsNullOrEmpty(context))
				return null;
			if (context.IndexOf("-") == -1)
				return context;
			return context.Split('-')[0];
		}

		string GetContextGroup(string context)
		{
			if (string.IsNullOrEmpty(context))
				return null;
			if (context.IndexOf("-") == -1)
				return null;
			return context.Split('-')[1];
		}

		void CreateContextOptions(string context)
		{
			var contexts = new List<SelectListItem>();
			contexts.Add(
				new SelectListItem 
				{ 
					Text = "Whole site", 
					Value = "", 
					Selected = context == "" 
				}
			);
			contexts.Add(
				new SelectListItem
				{
					Text = "Quran",
					Value = "Quran",
					Selected = context == "Quran"
				}
			);
			contexts.Add(
				new SelectListItem
				{
					Text = "Tafsirs etc",
					Value = "Tafsir",
					Selected = context == "Tafsir"
				}
			);
			contexts.Add(
				new SelectListItem
				{
					Text = "All hadiths",
					Value = "Hadith",
					Selected = context == "Hadith"
				}
			);

			foreach (var tafsir in SharedData.Document.TafsirDocument.Tafsirs
				.OrderBy(x => x.IsTafsir))
			{
				contexts.Add(
					new SelectListItem
					{
						Text = (tafsir.IsTafsir ? "Tafsir-" : "") + tafsir.Mufassir,
						Value = "Tafsir-" + tafsir.Code,
						Selected = context == "Tafsir-" + tafsir.Code
					}
				);
			}

			foreach (var hadithCollection in SharedData.Document.HadithDocument.Collections)
			{
				contexts.Add(
					new SelectListItem
					{
						Text = "Hadith-" + hadithCollection.Name,
						Value = "Hadith-" + hadithCollection.Code,
						Selected = context == "Hadith-" + hadithCollection.Code
					}
				);
			}

			ViewData["context"] = contexts;
		}

		List<Search_Result> ExecuteSearch(
			string q, 
			IEnumerable<KeyValuePair<string, string>> additionalFiltering)
		{
 			SanitizeQueryString(ref q);

			foreach (var additionalFilter in additionalFiltering)
				q = "+" + additionalFilter.Key + ":" + additionalFilter.Value + " " + q;

			int totalResults;
			var model = SearchEngine.Search(q, out totalResults, 500)
				.ToList()
				.ConvertAll(x =>
				{
					string url;
					string caption;
					string id;
					string[] idParts = x.ID.Split('/');
					switch (x.Type)
					{
						case "Quran":
							url = "/" + x.ID + "?allTranslations=y";
							id = x.ID;
							caption = "";//x.ID;
							break;

						case "Tafsir":
                            QuranX.DocumentModel.Tafsir tafsir;
                            SharedData.Document.TafsirDocument.TryGetTafsir(idParts[0], out tafsir);
                            url = "/Tafsir/" + x.ID.Split('-')[0];
                            caption = tafsir != null ? tafsir.Mufassir : idParts[0];
							id = idParts[1];
							break;

						case "Hadith":
							string hadithCollectionCode = idParts[0];
                            string indexCode = idParts[1];
							string[] hadithReferenceParts = idParts[2].Split('.');
							var hadithCollection = SharedData.Document.HadithDocument[hadithCollectionCode];
                            var referenceDefinition = hadithCollection.GetReferenceDefinition(indexCode);
							int referencePartIndex = 0;
							url = "";
							foreach (string referencePartName in referenceDefinition.PartNames)
								url += string.Format("{0}-{1}/", referencePartName, hadithReferenceParts[referencePartIndex++]);
							id = url
								.Replace("/", " ")
								.Replace("-", " ");
							url = "/Hadith/" + hadithCollection.Code + "/" + indexCode + "/" + url;
							caption = SharedData.Document.HadithDocument[hadithCollectionCode].Name;
							break;

						default:
							throw new NotImplementedException("Search doc type: " + x.Type);
					}
					return new Search_Result(
						type: x.Type,
						id: id,
						caption: caption,
						url: url,
						snippets: x.Snippets
					);
				}
			);
			ViewBag.TotalResults = totalResults;
			ViewBag.IsSearch = !string.IsNullOrEmpty(q);
			return model;
		}

		static void SanitizeQueryString(ref string queryString)
		{
			if (string.IsNullOrEmpty(queryString))
				return;
			queryString = ArabicHelper.Substitute(queryString);

			string[] specialCharacters = new string[] 
			{
				"&", "|", "!", "(", ")", "{", "}", "[", "]", 
				"^", "\"", "~", "?", ":", "\\"
			};
			foreach (string s in specialCharacters)
				queryString = queryString.Replace(s, @"\" + s);

			queryString = "+(" + queryString + ")";
		}


	}
}
