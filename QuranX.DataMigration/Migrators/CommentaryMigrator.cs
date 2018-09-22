using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentatorViewModel = QuranX.Persistence.Models.Commentator;
using XmlDocument = QuranX.DocumentModel.Document;
using NLog;
using QuranX.Persistence.Services.Repositories;
using QuranX.DataMigration.Services;
using QuranX.DocumentModel;

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

		public CommentaryMigrator(
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			ICommentatorWriteRepository commentatorWriteRepository)
		{
			Logger = logger;
			XmlDocument = xmlDocumentProvider.Document;
			CommentatorWriteRepository = commentatorWriteRepository;
		}

		public void Migrate()
		{
			foreach(Tafsir commentator in XmlDocument.TafsirDocument.Tafsirs)
			{
				MigratorCommentator(commentator);
			}
		}

		private void MigratorCommentator(Tafsir commentator)
		{
			Logger.Debug($"Commentary {commentator.Code}");
			var commentatorViewModel = new CommentatorViewModel(
				code: commentator.Code,
				description: commentator.Mufassir,
				isTafsir: commentator.IsTafsir);
			CommentatorWriteRepository.Write(commentatorViewModel);

			foreach(TafsirComment commentary in commentator.Comments)
			{
			}
		}
	}
}
