using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Views.Dictionaries;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class DictionariesController : Controller
	{
		private readonly IDictionaryRepository DictionaryRepository;
		private readonly IDictionaryEntryRepository DictionaryEntryRepository;

		public DictionariesController(IDictionaryRepository dictionaryRepository, IDictionaryEntryRepository dictionaryEntryRepository)
		{
			DictionaryRepository = dictionaryRepository ?? throw new ArgumentNullException(nameof(dictionaryRepository));
			DictionaryEntryRepository = dictionaryEntryRepository ?? throw new ArgumentNullException(nameof(dictionaryEntryRepository));
		}

		public ActionResult Index(string root)
		{
			IEnumerable<Persistence.Models.Dictionary> dictionaries = DictionaryRepository.GetAll();
			IEnumerable<string> nextRoots = DictionaryEntryRepository.GetNextRoots(root);
			IEnumerable<Persistence.Models.DictionaryEntry> dictionaryEntries = 
				string.IsNullOrWhiteSpace(root)
				? []
				: DictionaryEntryRepository.Get(root);
			var viewModel = new DictionaryListViewModel(
				currentRoot: root,
				childRoots: nextRoots,
				dictionaries: dictionaries,
				dictionaryEntries: dictionaryEntries.OrderBy(x => x.DictionaryCode));
			return View("DictionaryList", viewModel);
		}
	}
}
