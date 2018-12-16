using System.Collections.Generic;
using System.Web.Mvc;
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
				return HttpNotFound();
			word = ArabicHelper.Substitute(word);
			DictionaryEntry entry = DictionaryEntryRepository.Get(
				dictionaryCode: dictionaryCode, 
				word: word);
			if (entry == null)
				return HttpNotFound();

			var viewModel = new ViewModel(dictionary, entry);

			return View("DictionaryEntry", viewModel);
		}
	}
}