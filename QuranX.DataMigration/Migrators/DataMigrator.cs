using QuranX.DataMigration.Services;

namespace QuranX.DataMigration.Migrators
{
	public interface IDataMigrator
	{
		void Migrate();
	}

	public class DataMigrator : IDataMigrator
	{
		private readonly IQuranMigrator QuranMigrator;

		public DataMigrator(IQuranMigrator quranMigrator)
		{
			QuranMigrator = quranMigrator;
		}

		public void Migrate()
		{
			QuranMigrator.Migrate();
		}
	}
}
