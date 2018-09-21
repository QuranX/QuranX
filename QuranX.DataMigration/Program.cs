using System;
using QuranX.DataMigration.Migrators;
using Unity;

namespace QuranX.DataMigration
{
	class Program
	{
		static void Main(string[] args)
		{
			var container = new UnityContainer();
			Services.Registration.Register(container);
			Persistence.Services.Registration.Register(container);
			var dataMigrator = container.Resolve<IDataMigrator>();
			dataMigrator.Migrate();
			Console.WriteLine("Done");
			Console.ReadLine();
		}
	}
}
