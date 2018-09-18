using System.Collections.Generic;
using QuranX.Shared.Models;

namespace QuranX.Persistence.Services
{
	public interface IChapterRepository
	{
		Chapter Get(int chapterNumber);
		IEnumerable<Chapter> GetAll();
	}

	public class ChapterRepository : IChapterRepository
	{
		public Chapter Get(int chapterNumber) => QuranStructure.Chapter(chapterNumber);
		public IEnumerable<Chapter> GetAll() => QuranStructure.Chapters;
	}
}