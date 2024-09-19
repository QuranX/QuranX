using System;
using Microsoft.Extensions.DependencyInjection;
using QuranX.DataMigration.Migrators;

namespace QuranX.DataMigration
{
	class Program
	{
		static void Main(string[] args)
		{
			var services = new ServiceCollection();
			Services.Registration.Register(services);
			Persistence.Services.Registration.Register(services);

			var serviceProvider = services.BuildServiceProvider();
			var dataMigrator = serviceProvider.GetRequiredService<IDataMigrator>();
			dataMigrator.Migrate();
			Console.WriteLine("Done");
			Console.ReadLine();
		}
	}
}
