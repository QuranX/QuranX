﻿using CommentatorViewModel = QuranX.Persistence.Models.Commentator;
using CommentaryViewModel = QuranX.Persistence.Models.Commentary;
using XmlDocument = QuranX.DocumentModel.Document;
using NLog;
using QuranX.Persistence.Services.Repositories;
using QuranX.DataMigration.Services;
using QuranX.DocumentModel;
using System.Linq;
using QuranX.Persistence.Models;

namespace QuranX.DataMigration.Migrators
{
	public interface ICommentaryMigrator
	{
		void Migrate();
	}

	public class CommentaryMigrator : ICommentaryMigrator
	{
		private readonly XmlDocument XmlDocument;
		private readonly ILogger Logger;
		private readonly ICommentatorWriteRepository CommentatorWriteRepository;
		private readonly ICommentaryWriteRepository CommentaryWriteRepository;

		public CommentaryMigrator(
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			ICommentatorWriteRepository commentatorWriteRepository,
			ICommentaryWriteRepository commentaryWriteRepository)
		{
			Logger = logger;
			XmlDocument = xmlDocumentProvider.Document;
			CommentatorWriteRepository = commentatorWriteRepository;
			CommentaryWriteRepository = commentaryWriteRepository;
		}

		public void Migrate()
		{
			foreach (Tafsir commentator in XmlDocument.TafsirDocument.Tafsirs)
			{
				MigratorCommentator(commentator);
			}
		}

		private void MigratorCommentator(Tafsir commentator)
		{
			Logger.Debug($"Commentary {commentator.Code}");
			var commentatorViewModel = new CommentatorViewModel(
				code: commentator.Code,
				description: commentator.Mufassir);
			CommentatorWriteRepository.Write(commentatorViewModel);

			foreach (TafsirComment commentary in commentator.Comments)
			{
				var commentaryViewModel = new CommentaryViewModel(
					commentatorCode: commentator.Code,
					chapterNumber: commentary.VerseReference.Chapter,
					firstVerseNumber: commentary.VerseReference.FirstVerse,
					lastVerseNumber: commentary.VerseReference.LastVerse,
					text: commentary.Text.Select(x => TextContent.Create(x)));
				CommentaryWriteRepository.Write(commentaryViewModel);
			}
		}
	}
}
