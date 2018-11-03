using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Views.Search;

namespace QuranX.Web.Controllers
{
	public class SearchController : Controller
	{
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly IHadithCollectionRepository HadithCollectionRepository;

		public SearchController(
			ICommentatorRepository commentatorRepository,
			IHadithCollectionRepository hadithCollectionRepository)
		{
			CommentatorRepository = commentatorRepository;
			HadithCollectionRepository = hadithCollectionRepository;
		}

		public ActionResult Index(string q, string context)
		{
			context = (context ?? "").ToLowerInvariant();
			var contextItems = new List<SelectListItem>();
			contextItems.AddRange(CreateTopLevelSelection(context));
			contextItems.AddRange(CreateTafsirsSelection(context));
			contextItems.AddRange(CreateHadithsSelection(context));
			var viewModel = new ViewModel(q, contextItems);
			return View("Search", viewModel);
		}

		private IEnumerable<SelectListItem> CreateTopLevelSelection(string selectedValue)
		{
			var group = new SelectListGroup {
				Name = "Sections"
			};
			var result = new List<SelectListItem>();
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