using NLog;
using QuranX.DataMigration.Services;
using QuranX.Persistence.Services.Repositories;
using QuranX.DocumentModel;
using System.Linq;
using System.Collections.Generic;
using QuranX.Persistence.Extensions;
using HadithCollectionViewModel = QuranX.Persistence.Models.HadithCollection;
using XmlDocument = QuranX.DocumentModel.Document;
using HadithIndexDefinitionViewModel = QuranX.Persistence.Models.HadithReferenceDefinition;
using HadithViewModel = QuranX.Persistence.Models.Hadith;
using HadithReferenceViewModel = QuranX.Persistence.Models.HadithReference;
using System;

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
		private readonly IVerseRepository VerseRepository;
		private readonly IVerseWriteRepository VerseWriteRepository;
		private readonly IHadithCollectionWriteRepository HadithCollectionWriteRepository;
		private readonly IHadithWriteRepository HadithWriteRepository;
		private int NextHadithId;

		public HadithMigrator(
			ILogger logger,
			IXmlDocumentProvider xmlDocumentProvider,
			IVerseRepository verseRepository,
			IVerseWriteRepository verseWriteRepository,
			IHadithCollectionWriteRepository hadithCollectionWriteRepository,
			IHadithWriteRepository hadithWriteRepository)
		{
			Logger = logger;
			VerseRepository = verseRepository;
			VerseWriteRepository = verseWriteRepository;
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
					MigrateHadith(hadith, referenceDefinitions);
				}
			}
		}

		private void MigrateHadith(Hadith hadith, IEnumerable<HadithIndexDefinitionViewModel> referenceDefinitions)
		{
			Dictionary<string, HadithIndexDefinitionViewModel> definitionsByCode =
				referenceDefinitions.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);

			int hadithId = NextHadithId++;
			var references = new List<HadithReferenceViewModel>();
			HadithReferenceViewModel primaryReference = null;
			foreach (HadithReference hadithReference in hadith.References)
			{
				(int index, string suffix)[] indexValues =
					hadithReference.Values
					.Select(x => HadithReferenceViewModel.SplitValue(x))
					.ToArray();
				if (!string.IsNullOrWhiteSpace(hadithReference.Suffix))
					indexValues[indexValues.Length - 1].suffix = hadithReference.Suffix;
				var reference = new HadithReferenceViewModel(
					collectionCode: hadith.Collection.Code,
					referenceCode: hadithReference.Code,
					referenceValue1: indexValues[0].index,
					referenceValue1Suffix: indexValues[0].suffix.AsNullIfWhiteSpace(),
					referenceValue2: indexValues.Length > 1 ? indexValues[1].index : (int?)null,
					referenceValue2Suffix: indexValues.Length > 1 ? indexValues[1].suffix.AsNullIfWhiteSpace() : null,
					referenceValue3: indexValues.Length > 2 ? indexValues[2].index : (int?)null,
					referenceValue3Suffix: indexValues.Length > 2 ? indexValues[2].suffix.AsNullIfWhiteSpace() : null,
					hadithId: hadithId);
				references.Add(reference);

				var referenceDefinition = definitionsByCode[hadithReference.Code];
				if (primaryReference == null || referenceDefinition.IsPrimary)
					primaryReference = reference;
			}

			var primaryDefinition = definitionsByCode[primaryReference.ReferenceCode];
			string primaryIndexPath = string.Join("/", 
				primaryReference
					.ToNameValuePairs(primaryDefinition)
					.Select(x => $"{x.Key}-{x.Value}"));

			var hadithViewModel = new HadithViewModel(
				id: hadithId,
				collectionCode: hadith.Collection.Code,
				primaryReferenceCode: primaryReference.ReferenceCode,
				primaryReferencePath: primaryIndexPath,
				arabicText: hadith.ArabicText,
				englishText: hadith.EnglishText,
				verseRangeReferences: hadith.VerseReferences,
				references: references);
			HadithWriteRepository.Write(hadithViewModel);
		}
	}
}
