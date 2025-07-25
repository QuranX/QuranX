﻿using System.IO;
using Lucene.Net.Index;
using QuranX.Persistence.Services;

namespace QuranX.DataMigration.Migrators
{
	public interface IDataMigrator
	{
		void Migrate();
	}

	public class DataMigrator : IDataMigrator
	{
		private readonly ICorpusMigrator CorpusMigrator;
		private readonly IQuranMigrator QuranMigrator;
		private readonly ICommentaryMigrator CommentaryMigrator;
		private readonly IHadithMigrator HadithMigrator;
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;
		private readonly IDictionariesMigrator DictionariesMigrator;
		private readonly ISettings Settings;

		public DataMigrator(
			ICorpusMigrator corpusMigrator,
			IQuranMigrator quranMigrator, 
			ICommentaryMigrator commentaryMigrator, 
			IHadithMigrator hadithMigrator,
			ILuceneIndexWriterProvider indexWriterProvider,
			IDictionariesMigrator dictionariesMigrator,
			ISettings settings)
		{
			CorpusMigrator = corpusMigrator;
			QuranMigrator = quranMigrator;
			CommentaryMigrator = commentaryMigrator;
			HadithMigrator = hadithMigrator;
			IndexWriterProvider = indexWriterProvider;
			DictionariesMigrator = dictionariesMigrator;
			Settings = settings;
		}

		public void Migrate()
		{
			Directory.Delete(Settings.DataPath, true);
			Directory.CreateDirectory(Settings.DataPath);

			DictionariesMigrator.Migrate();
			CorpusMigrator.Migrate();
			QuranMigrator.Migrate();
			CommentaryMigrator.Migrate();
			HadithMigrator.Migrate();

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.Commit();
			indexWriter.ForceMerge(1, doWait: true);
		}
	}
}
