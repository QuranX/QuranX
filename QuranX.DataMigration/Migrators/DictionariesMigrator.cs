using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranX.DataMigration.Migrators
{
	public interface IDictionariesMigrator
	{
		void Migrate();
	}

	public class DictionariesMigrator : IDictionariesMigrator
	{
		private readonly ILisaanDictionaryMigrator LanesLexiconMigrator;

		public DictionariesMigrator(ILisaanDictionaryMigrator lanesLexiconMigrator)
		{
			LanesLexiconMigrator = lanesLexiconMigrator;
		}

		public void Migrate()
		{
			LanesLexiconMigrator.Migrate();
		}
	}
}
