using Microsoft.Extensions.DependencyInjection;
using QuranX.DataMigration.Migrators;
using System;

namespace QuranX.DataMigration;

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        Registration.Register(services);
        Persistence.Registration.Register(services);

        var serviceProvider = services.BuildServiceProvider();
        var dataMigrator = serviceProvider.GetRequiredService<IDataMigrator>();
        dataMigrator.Migrate();
        Console.WriteLine("Done");
        Console.ReadLine();
    }
}
