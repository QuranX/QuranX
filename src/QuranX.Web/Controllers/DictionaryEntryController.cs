using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;
using QuranX.Web.Views.DictionaryEntry;

namespace QuranX.Web.Controllers
{
	public class DictionaryEntryController : Controller
	{
		private readonly IDictionaryRepository DictionaryRepository;
		private readonly IDictionaryEntryRepository DictionaryEntryRepository;

		public DictionaryEntryController(
			IDictionaryRepository dictionaryRepository,
			IDictionaryEntryRepository dictionaryEntryRepository)
		{
			DictionaryRepository = dictionaryRepository;
			DictionaryEntryRepository = dictionaryEntryRepository;
		}

		public ActionResult Index(string dictionaryCode, string word)
		{
			Dictionary dictionary = DictionaryRepository.Get(dictionaryCode);
			if (dictionary == null)
				return NotFound();
			string indexValue = ArabicHelper.Substitute(word);
			IEnumerable<DictionaryEntry> entries = DictionaryEntryRepository.Get(
				dictionaryCode: dictionaryCode, 
				word: indexValue);
			if (!entries.Any())
				return NotFound();

			var viewModel = new ViewModel(word, dictionary, entries);

			return View("DictionaryEntry", viewModel);
		}
	}
}