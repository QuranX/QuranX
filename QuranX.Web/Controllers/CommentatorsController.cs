using System.Web.Mvc;
using QuranX.Persistence.Services.Repositories;

namespace QuranX.Web.Controllers
{
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
			return View(viewModel);
		}
	}
}