using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared;
using QuranX.Web.Views.DictionaryEntry;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Web.Controllers;

[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
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
        Dictionary dictionary = DictionaryRepository.Get(ref dictionaryCode);
        if (dictionary is null)
            return NotFound();
        string indexValue = ArabicHelper.SubstituteAndOmit(word);
        if (indexValue.Length != word.Length)
            return NotFound();

        IEnumerable<DictionaryEntry> entries = DictionaryEntryRepository.Get(
            dictionaryCode: dictionaryCode,
            word: indexValue);
        if (!entries.Any())
            return NotFound();

        ViewBag.Canonical = $"/Dictionary/{dictionaryCode}/{word}";
        var viewModel = new ViewModel(word, dictionary, entries);
        return View("DictionaryEntry", viewModel);
    }
}