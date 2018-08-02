using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuranX
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			RegisterSiteMapRoutes(routes);
			RegisterHadithsByVerseRoutes(routes);
			RegisterTafsirsByVerseRoutes(routes);
			RegisterQuranRoutes(routes);
			RegisterAnalysisRoutes(routes);
			RegisterTafsirRoutes(routes);
			RegisterHadithRoutes(routes);
			RegisterSearchRoutes(routes);
			RegisterMiscRoutes(routes);
		}

		static void RegisterMiscRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				name: "Help",
				url: "Help",
				defaults: new
				{
					Controller = "Help",
					Action = "Index"
				}
			);
			routes.MapRoute(
				name: "About",
				url: "About",
				defaults: new
				{
					Controller = "Home",
					Action = "About"
				}
			);
		}

		static void RegisterSearchRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				name: "Search",
				url: "Search",
				defaults: new
				{
					Controller = "Search",
					Action = "Index"
				}
			);

			routes.MapRoute(
				name: "",
				url: "Search/Help",
				defaults: new
				{
					Controller = "Search",
					Action = "Help"
				}
			);

		}

		static void RegisterSiteMapRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				name: "",
				url: "SiteMaps/Quran.xml",
				defaults: new
				{
					Controller = "SiteMap",
					Action = "Quran"
				}
			);

			routes.MapRoute(
				name: "",
				url: "SiteMaps/tafsir/{tafsirCode}-{PageIndex}.xml",
				defaults: new
				{
					Controller = "SiteMap",
					Action = "Tafsir"
				}
			);

			routes.MapRoute(
				name: "",
				url: "SiteMaps/hadith/{collectionCode}-{PageIndex}.xml",
				defaults: new
				{
					Controller = "SiteMap",
					Action = "Hadith"
				}
			);
		}

		static void RegisterHadithRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				name: "Hadiths",
				url: "Hadiths",
				defaults: new
				{
					Controller = "Hadith",
					Action = "Index"
				}
			);

			routes.MapRoute(
				name: "",
				url: "Hadith/RedirectToChapterVerse",
				defaults: new
				{
					Controller = "Hadith",
					Action = "RedirectToChapterVerse"
				}
			);

            routes.MapRoute(
                name: "",
                url: "Hadith/{CollectionCode}/{IndexCode}/{*Path}",
                defaults: new
                {
                    Controller = "Hadith",
                    Action = "Collection",
                    Path = UrlParameter.Optional
                }
            );
		}

		static void RegisterTafsirRoutes(RouteCollection routes)
		{
			routes.MapRoute(
				name: "Tafsirs",
				url: "Tafsirs",
				defaults: new
				{
					Controller = "Tafsir",
					Action = "Index"
				}
			);

			routes.MapRoute(
				name: "",
				url: "Tafsir/{Tafsir}",
				defaults: new
				{
					Controller = "Tafsir",
					Action = "CommentaryIndexes"
				}
			);


			routes.MapRoute(
				name: "",
				url: "Tafsir/{Tafsir}/{Chapter}.{Verse}",
				defaults: new
				{
					Controller = "Tafsir",
					Action = "Verse"
				},
				constraints: new
				{
					Chapter = @"\d+",
					Verse = @"\d+"
				}
			);

			routes.MapRoute(
					name: "",
					url: "Tafsir/{Tafsir}/RedirectToChapterVerse",
					defaults: new
					{
						Controller = "Tafsir",
						Action = "RedirectToChapterVerse"
					}
				);
		}

		static void RegisterTafsirsByVerseRoutes(RouteCollection routes)
		{
			routes.MapRoute(
					name: "",
					url: "Tafsirs/{Chapter}.{Verse}",
					defaults: new
					{
						Controller = "TafsirsByVerse",
						Action = "Verse"
					},
					constraints: new
					{
						Chapter = @"\d+",
						Verse = @"\d+"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Tafsirs/RedirectToChapterVerse",
					defaults: new
					{
						Controller = "TafsirsByVerse",
						Action = "RedirectToChapterVerse"
					}
				);
		}

		static void RegisterHadithsByVerseRoutes(RouteCollection routes)
		{
			routes.MapRoute(
					name: "",
					url: "Hadiths/{Chapter}.{Verse}",
					defaults: new
					{
						Controller = "HadithsByVerse",
						Action = "Verse"
					},
					constraints: new
					{
						Chapter = @"\d+",
						Verse = @"\d+"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Hadiths/RedirectToChapterVerse",
					defaults: new
					{
						Controller = "HadithsByVerse",
						Action = "RedirectToChapterVerse"
					}
				);
		}

        static void RegisterQuranRoutes(RouteCollection routes)
        {
            routes.MapRoute(
                    name: "Quran",
                    url: "",
                    defaults: new
                    {
                        Controller = "Quran",
                        Action = "Chapters",
                    }
                );

			routes.MapRoute(
					name: "",
					url: "{Chapter}.{Verse}-{LastVerse}",
					defaults: new
					{
						Controller = "Quran",
						Action = "Verse"
					},
					constraints: new
					{
						Chapter = @"\d+",
						Verse = @"\d+",
						LastVerse = @"\d+"
					}
				);

			routes.MapRoute(
					name: "",
					url: "{Chapter}.{Verse}",
					defaults: new
					{
						Controller = "Quran",
						Action = "Verse",
						LastVerse = -1
					},
					constraints: new
					{
						Chapter = @"\d+",
						Verse = @"\d+"
					}
				);

			routes.MapRoute(
					name: "",
					url: "{Chapter}",
					defaults: new
					{
						Controller = "Quran",
						Action = "Verse",
						Verse = 1,
						LastVerse = -1
					},
					constraints: new
					{
						Chapter = @"\d+"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Quran/RedirectToChapter",
					defaults: new
					{
						Controller = "Quran",
						Action = "RedirectToChapter"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Quran/RedirectToChapterVerse",
					defaults: new
					{
						Controller = "Quran",
						Action = "RedirectToChapterVerse"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Quran/GetVersesView",
					defaults: new
					{
						Controller = "Quran",
						Action = "GetVersesView"
					}
				);

			routes.MapRoute(
				name: "",
				url: "{*Verses}",
				defaults: new
				{
					Controller = "Quran",
					Action = "MultipleVerses"
				},
				constraints: new
				{
					Verses = @"^(\d+\.\d+(-\d+)?)(,(\d+\.\d+(-\d+)?))*$"
				}
			);
		}

		static void RegisterAnalysisRoutes(RouteCollection routes)
		{
			routes.MapRoute(
					name: "",
					url: "Analysis/{Chapter}.{Verse}",
					defaults: new
					{
						Controller = "Analysis",
						Action = "Verse"
					},
					constraints: new
					{
						Chapter = @"\d+",
						Verse = @"\d+"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Analysis/RedirectToChapterVerse",
					defaults: new
					{
						Controller = "Analysis",
						Action = "RedirectToChapterVerse"
					}
				);

			routes.MapRoute(
					name: "",
					url: "Analysis/Root/{Root}",
					defaults: new
					{
						Controller = "Analysis",
						Action = "Root"
					}
				);

		}

	}
}