using NLog;
using QuranX.DataMigration.Services;
using QuranX.Persistence.Services.Repositories;
using QuranX.DocumentModel;
using HadithCollectionViewModel = QuranX.Persistence.Models.HadithCollection;
using XmlDocument = QuranX.DocumentModel.Document;
using HadithIndexDefinitionViewModel = QuranX.Persistence.Models.HadithIndexDefinition;
using System.Linq;

namespace QuranX.DataMigration.Migrators
{
	public interface IHadithMigrator
	{
		void Migrate();
	}

	public class HadithMigrator : IHadithMigrator
	{
		private readonly XmlDocument XmlDocument;
		private readonly ILogger Logger;
		private readonly IHadithCollectionWriteRepository HadithCollectionWriterRepository;

		public HadithMigrator(
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			IHadithCollectionWriteRepository hadithCollectionWriterRepository)
		{
			Logger = logger;
			XmlDocument = xmlDocumentProvider.Document;
			HadithCollectionWriterRepository = hadithCollectionWriterRepository;
		}


		public void Migrate()
		{
			foreach (HadithCollection collection in XmlDocument.HadithDocument.Collections)
			{
				Logger.Debug($"Hadith collection {collection.Name}");
				var indexDefinitions = collection
					.ReferenceDefinitions
					.Select(x => new HadithIndexDefinitionViewModel(
						collectionCode: collection.Code,
						code: x.Code,
						name: x.Name,
						valuePrefix: x.ValuePrefix,
						partNames: x.PartNames,
						isPrimary: x.IsPrimary))
					.ToArray();
				var collectionViewModel = new HadithCollectionViewModel(
					code: collection.Code,
					name: collection.Name,
					indexDefinitions: indexDefinitions,
					hadithCount: collection.Hadiths.Count());
				HadithCollectionWriterRepository.Write(collectionViewModel);
			}
		}
	}
}
