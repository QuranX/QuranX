using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class TafsirDocumentVisitor
	{
		protected virtual void VisitDocument(TafsirDocument document)
		{
			VisitTafsirs(document.Tafsirs);
		}

		protected virtual void VisitTafsirs(IEnumerable<Tafsir> tafsirs)
		{
			foreach (var tafsir in tafsirs)
				VisitTafsir(tafsir);
		}

		protected virtual void VisitTafsir(Tafsir tafsir)
		{
			var commentariesByChapter = tafsir.Comments
				.GroupBy(x => x.VerseReference.Chapter);
			VisitChapters(commentariesByChapter);
		}

		protected virtual void VisitChapters(IEnumerable<IGrouping<int, TafsirComment>> commentariesByChapter)
		{
			foreach (var chapter in commentariesByChapter.OrderBy(x => x.Key))
				VisitChapter(chapter.Key, chapter);
		}

		protected virtual void VisitChapter(int chapterIndex, IEnumerable<TafsirComment> comments)
		{
			VisitComments(chapterIndex, comments.OrderBy(x => x.VerseReference));
		}

		protected virtual void VisitComments(int chapterIndex, IEnumerable<TafsirComment> comments)
		{
			foreach (var comment in comments)
				VisitComment(comment);
		}

		protected virtual void VisitComment(TafsirComment comment)
		{
		}
	}
}
