using System;

namespace QuranX.DataMigration.Migrators
{
	public interface ILanesLexiconMigrator
	{
		void Migrate();
	}

	public class LanesLexiconMigrator : ILanesLexiconMigrator
	{
		public void Migrate()
		{
			throw new NotImplementedException();
		}
	}
}
