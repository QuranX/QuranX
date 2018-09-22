using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Models;

namespace QuranX.Web.Controllers
{
	public class CommentaryController : Controller
	{
		private readonly IChapterRepository ChapterRepository;
		private readonly ICommentaryRepository CommentaryRepository;

		public CommentaryController(IChapterRepository chapterRepository, ICommentaryRepository commentaryRepository)
		{
			ChapterRepository = chapterRepository;
			CommentaryRepository = commentaryRepository;
		}

		public ActionResult Index(string commentatorCode)
		{
			Dictionary<int, Chapter> chapterByIndex =
				ChapterRepository.GetAll().ToDictionary(x => x.ChapterNumber);
			VerseRangeReference[] verseRangeReferences = CommentaryRepository.GetVerseRangeReferences(commentatorCode);
			ChapterAndVerseRangeReferenceSelection[] viewModel = verseRangeReferences
				.GroupBy(x => x.Chapter)
				.Select(x => new ChapterAndVerseRangeReferenceSelection(
					chapter: chapterByIndex[x.Key],
					verseRangeReferences: x.OrderBy(v => v.FirstVerse)))
				.OrderBy(x => x.Chapter.ChapterNumber)
				.ToArray();
			return View();
		}
	}
}