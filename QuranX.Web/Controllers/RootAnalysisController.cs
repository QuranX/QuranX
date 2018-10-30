using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;

namespace QuranX.Web.Controllers
{
	public class RootAnalysisController : Controller
	{
		private readonly IVerseAnalysisRepository VerseAnalysisWordRepository;

		public RootAnalysisController(
			IVerseAnalysisRepository verseAnalysisWordRepository)
		{
			VerseAnalysisWordRepository = verseAnalysisWordRepository;
		}

		public ActionResult Index(string root)
		{
			root = ArabicHelper.LetterNamesToArabic(root);
			throw new NotImplementedException();
		}
	}
}