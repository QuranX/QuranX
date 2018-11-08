using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Views.RootAnalysis;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class RootAnalysisController : Controller
	{
		private readonly IVerseAnalysisRepository VerseAnalysisRepository;

		public RootAnalysisController(
			IVerseAnalysisRepository verseAnalysisRepository)
		{
			VerseAnalysisRepository = verseAnalysisRepository;
		}

		public ActionResult Index(string rootLetterNames)
		{
			string root = ArabicHelper.LetterNamesToArabic(rootLetterNames);
			IEnumerable<VerseAnalysis> verses =
				VerseAnalysisRepository.GetForRoot(root)
				.OrderBy(x => x.ChapterNumber)
				.ThenBy(x => x.VerseNumber);
			var extracts = new List<VerseViewModel>();
			foreach (VerseAnalysis verseAnalysis in verses)
			{
				foreach (VerseAnalysisWord analysisWord in verseAnalysis.Words)
				{
					VerseAnalysisWordPart wordPart =
						analysisWord.WordParts
						.SingleOrDefault(x => x.Root == root);
					if (wordPart != null)
					{
						VerseViewModel extract =
							CreateVerseViewModel(verseAnalysis, analysisWord, wordPart);
						extracts.Add(extract);
					}
				}
			}

			var viewModel = new ViewModel(
				arabicRoot: root,
				rootLetterNames: rootLetterNames,
				extracts: extracts);
			return View("RootAnalysis", viewModel);
		}

		private VerseViewModel CreateVerseViewModel(
			VerseAnalysis verseAnalysis,
			VerseAnalysisWord analysisWord,
			VerseAnalysisWordPart analysisWordPart)
		{
			const int WordsBeforeAndAfter = 3;
			int lower = Math.Max(1, analysisWord.WordNumber - WordsBeforeAndAfter);
			int upper = Math.Min(verseAnalysis.Words.Count, analysisWord.WordNumber + WordsBeforeAndAfter);
			var words = new List<VerseAnalysisWord>();
			for (int index = lower - 1; index < upper; index++)
				words.Add(verseAnalysis.Words[index]);

			var result = new VerseViewModel(
				chapterNumber: verseAnalysis.ChapterNumber,
				verseNumber: verseAnalysis.VerseNumber,
				selectedWord: analysisWord,
				selectedWordPart: analysisWordPart,
				words: words);
			return result;
		}
	}
}