using System.Web.Mvc;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.Controllers
{
	[OutputCache(Duration = Consts.CacheTimeInSeconds, NoStore = Consts.CacheTimeInSeconds == 0)]
	public class CommentatorsController : Controller
	{
		private readonly ICommentatorRepository CommentatorRepository;

		public CommentatorsController(ICommentatorRepository commentatorRepository)
		{
			CommentatorRepository = commentatorRepository;
		}

		public ActionResult Index()
		{
			var viewModel = CommentatorRepository.GetAll();
			return View("Commentators", viewModel);
		}
	}
}