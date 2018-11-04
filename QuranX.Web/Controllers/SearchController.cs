using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Services;
using QuranX.Web.Views.Search;

namespace QuranX.Web.Controllers
{
	public class SearchController : Controller
	{
		private readonly ISearchEngine SearchEngine;
		private readonly ISearchResultWithLinkFactory SearchResultWithLinkFactory;
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public SearchController(
			ISearchEngine searchEngine,
			ISearchResultWithLinkFactory searchResultWithLinkFactory,
			ICommentatorRepository commentatorRepository,
			IHadithCollectionRepository hadithCollectionRepository)
		{
			SearchEngine = searchEngine;
			SearchResultWithLinkFactory = searchResultWithLinkFactory;
			CommentatorRepository = commentatorRepository;
			HadithCollectionRepository = hadithCollectionRepository;
		}

		public ActionResult Index(string q, string context)
		{
			context = (context ?? "").ToLowerInvariant();
			IEnumerable<SearchResultWithLink> searchResultsWithLink = null;
			if (!string.IsNullOrWhiteSpace(q))
			{
				IEnumerable<SearchResult> searchResults = SearchEngine.Search(q, out int totalResults);
				searchResultsWithLink =
					searchResults.Select(SearchResultWithLinkFactory.Create);
			}
			List<SelectListItem> contextItems = CreateContextItems(context);
			var viewModel = new ViewModel(q, contextItems, searchResultsWithLink);
			return View("Search", viewModel);
		}

		private List<SelectListItem> CreateContextItems(string context)
		{
			var contextItems = new List<SelectListItem>();
			contextItems.AddRange(CreateTopLevelSelection(context));
			contextItems.AddRange(CreateTafsirsSelection(context));
			contextItems.AddRange(CreateHadithsSelection(context));
			return contextItems;
		}

		private IEnumerable<SelectListItem> CreateTopLevelSelection(string selectedValue)
		{
			var group = new SelectListGroup {
				Name = "Sections"
			};
			var result = new List<SelectListItem>();
			result.Add(CreateSelectionItem(group, selectedValue, "", "Whole site"));
			result.Add(CreateSelectionItem(group, selectedValue, "quran", "Quran"));
			result.Add(CreateSelectionItem(group, selectedValue, "tafsir", "All commentaries"));
			result.Add(CreateSelectionItem(group, selectedValue, "hadith", "All hadiths"));
			return result;
		}


		private IEnumerable<SelectListItem> CreateTafsirsSelection(string selectedValue)
		{
			var group = new SelectListGroup {
				Name = "Commentary"
			};
			var result = new List<SelectListItem>();
			IEnumerable<Commentator> commentators =
				CommentatorRepository.GetAll()
				.OrderBy(x => x.Code);
			foreach (Commentator commentator in commentators)
			{
				string code = "tafsir-" + commentator.Code.ToLowerInvariant();
				var item = new SelectListItem {
					Group = group,
					Selected = (selectedValue == code),
					Value = code,
					Text = $"{commentator.Code} ({commentator.Description})"
				};
				result.Add(item);
			}
			return result;
		}

		private IEnumerable<SelectListItem> CreateHadithsSelection(string selectedValue)
		{
			var group = new SelectListGroup {
				Name = "Hadith"
			};
			var result = new List<SelectListItem>();
			IEnumerable<HadithCollection> collections =
				HadithCollectionRepository.GetAll()
				.OrderBy(x => x.Code);
			foreach (HadithCollection collection in collections)
			{
				string code = "hadith-" + collection.Code.ToLowerInvariant();
				var item = new SelectListItem {
					Group = group,
					Selected = (selectedValue == code),
					Value = code,
					Text = $"{collection.Code} ({collection.Name})"
				};
				result.Add(item);
			}
			return result;
		}

		private SelectListItem CreateSelectionItem(SelectListGroup group, string selectedValue, string code, string text)
		{
			return new SelectListItem {
				Group = group,
				Text = text,
				Value = code,
				Selected = code == selectedValue
			};
		}
	}
}