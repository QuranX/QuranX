using NLog;
using QuranX.DataMigration.Services;
using QuranX.Persistence.Services.Repositories;
using QuranX.DocumentModel;
using HadithCollectionViewModel = QuranX.Persistence.Models.HadithCollection;
using XmlDocument = QuranX.DocumentModel.Document;
using HadithIndexDefinitionViewModel = QuranX.Persistence.Models.HadithReferenceDefinition;
using HadithViewModel = QuranX.Persistence.Models.Hadith;
using HadithReferenceViewModel = QuranX.Persistence.Models.HadithReference;
using System.Linq;
using System.Collections.Generic;

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
		private readonly IHadithCollectionWriteRepository HadithCollectionWriteRepository;
		private readonly IHadithWriteRepository HadithWriteRepository;
		private int NextHadithId;

		public HadithMigrator(
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			IHadithCollectionWriteRepository hadithCollectionWriteRepository,
			IHadithWriteRepository hadithWriteRepository)
		{
			Logger = logger;
			XmlDocument = xmlDocumentProvider.Document;
			HadithCollectionWriteRepository = hadithCollectionWriteRepository;
			HadithWriteRepository = hadithWriteRepository;
		}


		public void Migrate()
		{
			NextHadithId = 1;
			foreach (HadithCollection collection in XmlDocument.HadithDocument.Collections)
			{
				Logger.Debug($"Hadith collection {collection.Name}");
				var referenceDefinitions = collection
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
					referenceDefinitions: referenceDefinitions,
					hadithCount: collection.Hadiths.Count());
				HadithCollectionWriteRepository.Write(collectionViewModel);

				foreach (Hadith hadith in collection.Hadiths)
				{
					MigrateHadith(hadith);
				}
			}
		}

		private void MigrateHadith(Hadith hadith)
		{
			int hadithId = NextHadithId++;
			IEnumerable<HadithReferenceViewModel> references = hadith.References
				.Select(x => new HadithReferenceViewModel(
					collectionCode: hadith.Collection.Code,
					indexCode: x.Code,
					indexPart1: x.Values.Length > 0 ? x.Values[0] : null,
					indexPart2: x.Values.Length > 1 ? x.Values[1] : null,
					indexPart3: x.Values.Length > 2 ? x.Values[2] : null,
					suffix: x.Suffix,
					hadithId: hadithId));
			var hadithViewModel = new HadithViewModel(
				id: hadithId,
				arabicText: hadith.ArabicText,
				englishText: hadith.EnglishText,
				verseRangeReferences: hadith.VerseReferences,
				references: references);
			HadithWriteRepository.Write(hadithViewModel);
		}
	}
}
