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
using System.Text.RegularExpressions;
using System;
using QuranX.Persistence.Extensions;

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
			var references = new List<HadithReferenceViewModel>();
			foreach(HadithReference hadithReference in hadith.References)
			{
				(int index, string suffix)[] indexValues =
					hadithReference.Values
					.Select(x => HadithReferenceViewModel.SplitValue(x))
					.ToArray();
				if (!string.IsNullOrWhiteSpace(hadithReference.Suffix))
					indexValues[indexValues.Length - 1].suffix = hadithReference.Suffix;
				var reference = new HadithReferenceViewModel(
					collectionCode: hadith.Collection.Code,
					indexCode: hadithReference.Code,
					indexPart1: indexValues[0].index,
					indexPart1Suffix: indexValues[0].suffix.AsNullIfWhiteSpace(),
					indexPart2: indexValues.Length > 1 ? indexValues[1].index : (int?)null,
					indexPart2Suffix: indexValues.Length > 1 ? indexValues[1].suffix.AsNullIfWhiteSpace() : null,
					indexPart3: indexValues.Length > 2 ? indexValues[2].index : (int?)null,
					indexPart3Suffix: indexValues.Length > 2 ? indexValues[2].suffix.AsNullIfWhiteSpace() : null,
					hadithId: hadithId);
				references.Add(reference);
			}
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
