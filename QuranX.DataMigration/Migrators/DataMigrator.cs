using System.IO;
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
		private readonly IQuranMigrator QuranMigrator;
		private readonly ICommentaryMigrator CommentaryMigrator;
		private readonly IHadithMigrator HadithMigrator;
		private readonly ILuceneIndexWriterProvider IndexWriterProvider;
		private readonly ISettings Settings;

		public DataMigrator(
			IQuranMigrator quranMigrator, 
			ICommentaryMigrator commentaryMigrator, 
			IHadithMigrator hadithMigrator,
			ILuceneIndexWriterProvider indexWriterProvider,
			ISettings settings)
		{
			QuranMigrator = quranMigrator;
			CommentaryMigrator = commentaryMigrator;
			HadithMigrator = hadithMigrator;
			IndexWriterProvider = indexWriterProvider;
			Settings = settings;
		}

		public void Migrate()
		{
			Directory.Delete(Settings.DataPath, true);
			Directory.CreateDirectory(Settings.DataPath);

			QuranMigrator.Migrate();
			CommentaryMigrator.Migrate();
			HadithMigrator.Migrate();

			IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
			indexWriter.Commit();
			indexWriter.Optimize(doWait: true);
		}
	}
}
