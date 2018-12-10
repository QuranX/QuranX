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
	public class SiteSearchController : Controller
	{
		private readonly ISearchEngine SearchEngine;
		private readonly ISearchResultWithLinkFactory SearchResultWithLinkFactory;
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public SiteSearchController(
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
			string subContext = null;

			string[] contextParts = context.Split('-');
			if (contextParts.Length == 2 && !string.IsNullOrWhiteSpace(contextParts[1]))
			{
				context = contextParts[0];
				subContext = contextParts[1];
			}

			int totalResults = 0;
			IEnumerable<SearchResultWithLink> searchResultsWithLink = null;
			if (!string.IsNullOrWhiteSpace(q))
			{
				IEnumerable<SearchResult> searchResults =
					SearchEngine.Search(q, context, subContext, out totalResults);
				searchResultsWithLink =
					searchResults.Select(SearchResultWithLinkFactory.Create);
			}
			List<SelectListItem> contextItems = CreateContextItems(context);
			var viewModel = new ViewModel(q, contextItems, searchResultsWithLink, totalResults);
			return View("SiteSearch", viewModel);
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
				Name = SearchContextGroupNames.Sections
			};
			var result = new List<SelectListItem>();
			result.Add(CreateSelectionItem(group, selectedValue, SearchContexts.WholeSite, "Whole site"));
			result.Add(CreateSelectionItem(group, selectedValue, SearchContexts.Quran, "Qur'an"));
			result.Add(CreateSelectionItem(group, selectedValue, SearchContexts.Commentaries, "All commentaries"));
			result.Add(CreateSelectionItem(group, selectedValue, SearchContexts.Hadiths, "All hadiths"));
			return result;
		}


		private IEnumerable<SelectListItem> CreateTafsirsSelection(string selectedValue)
		{
			var group = new SelectListGroup {
				Name = SearchContextGroupNames.Commentaries
			};
			var result = new List<SelectListItem>();
			IEnumerable<Commentator> commentators =
				CommentatorRepository.GetAll()
				.OrderBy(x => x.Code);
			foreach (Commentator commentator in commentators)
			{
				string code = $"{SearchContexts.Commentaries}-{commentator.Code}";
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
				Name = SearchContextGroupNames.Hadiths
			};
			var result = new List<SelectListItem>();
			IEnumerable<HadithCollection> collections =
				HadithCollectionRepository.GetAll()
				.OrderBy(x => x.Code);
			foreach (HadithCollection collection in collections)
			{
				string code = $"{SearchContexts.Hadiths}-{collection.Code}";
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