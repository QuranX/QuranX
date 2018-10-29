using System.Web.Mvc;
using System.Web.Routing;
using QuranX.Web.Controllers;

namespace QuranX.Web
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Commentators",
				url: "Tafsirs",
				defaults: new { Controller = "Commentators", Action = "Index" });

			routes.MapRoute(
				name: "CommentariesForVerse",
				url: "Tafsirs/{ChapterNumber}.{VerseNumber}",
				defaults: new { Controller = "CommentariesForVerse", Action = "Index" },
				constraints: new { ChapterNumber = @"\d+", VerseNumber = @"\d+" });

			routes.MapRoute(
				name: "VerseAnalysis",
				url: "Analysis/{ChapterNumber}.{VerseNumber}",
				defaults: new { Controller = "VerseAnalysis", Action = "Index" },
				constraints: new { ChapterNumber = @"\d+", VerseNumber = @"\d+" });

			//TODO: Write controller
			routes.MapRoute(
				name: "RootAnalysis",
				url: "Analysis/Root/{root}",
				defaults: new { Controller = "RootAnalysis", Action = "Index" }); ;

			//TODO: Write controller
			routes.MapRoute(
				name: "WordAnalysis",
				url: "Analysis/Word/{word}",
				defaults: new { Controller = "WordAnalysis", Action = "Index" }); ;

			routes.MapRoute(
				name: "VerseCommentary",
				url: "Tafsir/{CommentatorCode}/{ChapterNumber}.{VerseNumber}",
				defaults: new { Controller = "VerseCommentary", Action = "Index" },
				constraints: new { ChapterNumber = @"\d+", VerseNumber = @"\d+" });

			routes.MapRoute(
				name: "HadithCollections",
				url: "Hadiths",
				defaults: new { Controller = "HadithCollections", Action = "Index" });

			routes.MapRoute(
				name: "VerseHadiths",
				url: "Hadiths/{ChapterNumber}.{VerseNumber}",
				defaults: new { Controller = "VerseHadiths", Action = "Index" },
				constraints: new { ChapterNumber = @"\d+", VerseNumber = @"\d+" });

			routes.MapRoute(
				name: "HadithIndex3",
				url: "Hadith/{CollectionCode}/{ReferenceCode}/{ReferenceValue1}/{ReferenceValue2}/{ReferenceValue3}",
				defaults: new { Controller = "HadithIndex", Action = "Index" },
				constraints: new
				{
					ReferenceValue1 = @"([a-z]+)-(\d+)([a-z]*)",
					ReferenceValue2 = @"([a-z]+)-(\d+)([a-z]*)",
					ReferenceValue3 = @"([a-z]+)-(\d+)([a-z]*)"
				});

			routes.MapRoute(
				name: "HadithIndex2",
				url: "Hadith/{CollectionCode}/{ReferenceCode}/{ReferenceValue1}/{ReferenceValue2}",
				defaults: new { Controller = "HadithIndex", Action = "Index" },
				constraints: new
				{
					ReferenceValue1 = @"([a-z]+)-(\d+)([a-z]*)",
					ReferenceValue2 = @"([a-z]+)-(\d+)([a-z]*)"
				});

			routes.MapRoute(
				name: "HadithIndex1",
				url: "Hadith/{CollectionCode}/{ReferenceCode}/{ReferenceValue1}",
				defaults: new { Controller = "HadithIndex", Action = "Index" },
				constraints: new
				{
					ReferenceValue1 = @"([a-z]+)-(\d+)([a-z]*)"
				});

			routes.MapRoute(
				name: "HadithIndex",
				url: "Hadith/{CollectionCode}/{ReferenceCode}",
				defaults: new { Controller = "HadithIndex", Action = "Index" });

			routes.MapRoute(
				name: "QuranVerses",
				url: "{*Verses}",
				defaults: new { Controller = "QuranVerses", Action = "Index" },
				constraints: new
				{
					Verses = @"^(\d+\.\d+(-\d+)?)(,(\d+\.\d+(-\d+)?))*$"
				});

			routes.MapRoute(
				name: "Home",
				url: "",
				defaults: new
				{
					Controller = "Redirect",
					Action = nameof(RedirectController.Index),
					Url = "/1.1"
				});
		}
	}
}
