using System.Collections.Generic;
using System;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel
{
	public class Tafsir
	{
		readonly Dictionary<int, List<TafsirComment>> CommentsByChapter;
		readonly Dictionary<VerseReference, TafsirComment> CommentsByVerse;
		readonly List<TafsirComment> _Comments;
		public readonly string Code;
		public readonly bool IsTafsir;
		public readonly string Mufassir;
		public readonly string Copyright;

		public Tafsir(
			string code,
			string mufassir,
			bool isTafsir,
			string copyright)
		{
			this.Code = code;
			this.Mufassir = mufassir;
			this.IsTafsir = isTafsir;
			this.Copyright = copyright;
			this.CommentsByChapter = new Dictionary<int, List<TafsirComment>>();
			this.CommentsByVerse = new Dictionary<VerseReference, TafsirComment>();
			this._Comments = new List<TafsirComment>();
		}

		public void AddComment(TafsirComment comment)
		{
			_Comments.Add(comment);
			FindCommentariesByChapterEntry(chapter: comment.VerseReference.Chapter, createIfNotExists: true).Add(comment);
			for (int verseIndex = comment.VerseReference.FirstVerse;
				verseIndex <= comment.VerseReference.LastVerse;
				verseIndex++)
			{
				var verseReference = new VerseReference(
						chapter: comment.VerseReference.Chapter,
						verse: verseIndex
					);
				if (CommentsByVerse.ContainsKey(verseReference))
				{
					Console.WriteLine("Duplicate {0} {1}", this.Code, verseReference);
				}
				CommentsByVerse[verseReference] = comment;
			}
		}

		public IEnumerable<TafsirComment> Comments
		{
			get
			{
				return _Comments
					.OrderBy(x => x.VerseReference);
			}
		}

		public IEnumerable<TafsirComment> CommentariesForChapter(int chapterIndex)
		{
			var result = FindCommentariesByChapterEntry(chapter: chapterIndex, createIfNotExists: false);
			if (result == null)
				return new List<TafsirComment>();
			return result.OrderBy(x => x.VerseReference);
		}

		public TafsirComment CommentaryForVerse(int chapterIndex, int verseIndex)
		{
			TafsirComment commentary;
			var reference = new VerseReference(
					chapter: chapterIndex,
					verse: verseIndex
				);
			CommentsByVerse.TryGetValue(reference, out commentary);
			return commentary;
		}

		List<TafsirComment> FindCommentariesByChapterEntry(int chapter, bool createIfNotExists)
		{
			List<TafsirComment> list;
			if (!CommentsByChapter.TryGetValue(chapter, out list))
				if (createIfNotExists)
				{
					list = new List<TafsirComment>();
					CommentsByChapter[chapter] = list;
				}
			return list;
		}

	}
}
