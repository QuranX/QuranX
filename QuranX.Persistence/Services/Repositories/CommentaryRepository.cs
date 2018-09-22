using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuranX.Persistence.Models;

namespace QuranX.Persistence.Services.Repositories
{
	public interface ICommentaryRepository
	{
		Commentary[] GetForVerse(int chapterNumber, int verseNumber);
	}

	public class CommentaryRepository : ICommentaryRepository
	{
		public Commentary[] GetForVerse(int chapterNumber, int verseNumber)
		{
			throw new NotImplementedException();
		}
	}
}
