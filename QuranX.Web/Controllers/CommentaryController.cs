using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;
using QuranX.Web.Views.Commentary;

namespace QuranX.Web.Controllers
{
	public class CommentaryController : Controller
	{
		private readonly ICommentatorRepository CommentatorRepository;
		private readonly IChapterRepository ChapterRepository;
		private readonly ICommentaryRepository CommentaryRepository;

		public CommentaryController(
			ICommentatorRepository commentatorRepository,
			IChapterRepository chapterRepository, 
			ICommentaryRepository commentaryRepository)
		{
			CommentatorRepository = commentatorRepository;
			ChapterRepository = chapterRepository;
			CommentaryRepository = commentaryRepository;
		}

		public ActionResult Index(string commentatorCode)
		{
			Dictionary<int, Chapter> chapterByIndex =
				ChapterRepository.GetAll().ToDictionary(x => x.ChapterNumber);
			VerseRangeReference[] verseRangeReferences = CommentaryRepository.GetVerseRangeReferences(commentatorCode);
			ChapterAndVerseRangeReferenceSelection[] chaptersAndVerseRanges = verseRangeReferences
				.GroupBy(x => x.Chapter)
				.Select(x => new ChapterAndVerseRangeReferenceSelection(
					chapter: chapterByIndex[x.Key],
					verseRangeReferences: x.OrderBy(v => v.FirstVerse)))
				.OrderBy(x => x.Chapter.ChapterNumber)
				.ToArray();
			Commentator commentator = CommentatorRepository.Get(commentatorCode);
			ViewModel viewModel = new ViewModel(
				commentator: commentator,
				chapters: chaptersAndVerseRanges);
			return View(viewModel);
		}
	}
}