using NLog;
using QuranX.DataMigration.Services;
using QuranX.DocumentModel;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using HadithCollectionViewModel = QuranX.Persistence.Models.HadithCollection;
using HadithIndexDefinitionViewModel = QuranX.Persistence.Models.HadithReferenceDefinition;
using HadithReferenceViewModel = QuranX.Persistence.Models.HadithReference;
using HadithViewModel = QuranX.Persistence.Models.Hadith;
using XmlDocument = QuranX.DocumentModel.Document;

namespace QuranX.DataMigration.Migrators;

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

        // First pass: build references and identify primary to compute PrimaryReferencePath
        var rawReferences = new List<(HadithReference source, (int index, string suffix)[] indexValues)>();
        HadithReference primarySource = null;
        (int index, string suffix)[] primaryIndexValues = null;
        foreach (HadithReference hadithReference in hadith.References)
        {
            (int index, string suffix)[] indexValues =
                hadithReference.Values
                .Select(x => HadithReferenceViewModel.SplitValue(x))
                .ToArray();
            if (!string.IsNullOrWhiteSpace(hadithReference.Suffix))
                indexValues[indexValues.Length - 1].suffix = hadithReference.Suffix;
            rawReferences.Add((hadithReference, indexValues));

            var referenceDefinition = definitionsByCode[hadithReference.Code];
            if (primarySource is null || referenceDefinition.IsPrimary)
            {
                primarySource = hadithReference;
                primaryIndexValues = indexValues;
            }
        }

        // Compute primary reference path
        var primaryDefinition = definitionsByCode[primarySource.Code];
        string primaryIndexPath = string.Join("/",
            primaryDefinition.PartNames.Select((partName, i) =>
                $"{partName}-{primaryIndexValues[i].index}{primaryIndexValues[i].suffix}"));

        // Second pass: create reference view models with the primary reference path
        var references = new List<HadithReferenceViewModel>();
        foreach (var (source, indexValues) in rawReferences)
        {
            var reference = new HadithReferenceViewModel(
                collectionCode: hadith.Collection.Code,
                referenceCode: source.Code,
                referenceValue1: indexValues[0].index,
                referenceValue1Suffix: indexValues[0].suffix.AsNullIfWhiteSpace(),
                referenceValue2: indexValues.Length > 1 ? indexValues[1].index : (int?)null,
                referenceValue2Suffix: indexValues.Length > 1 ? indexValues[1].suffix.AsNullIfWhiteSpace() : null,
                referenceValue3: indexValues.Length > 2 ? indexValues[2].index : (int?)null,
                referenceValue3Suffix: indexValues.Length > 2 ? indexValues[2].suffix.AsNullIfWhiteSpace() : null,
                primaryReferencePath: primaryIndexPath);
            references.Add(reference);
        }

        var hadithViewModel = new HadithViewModel(
            collectionCode: hadith.Collection.Code,
            primaryReferenceCode: primarySource.Code,
            primaryReferencePath: primaryIndexPath,
            arabicText: hadith.ArabicText,
            englishText: hadith.EnglishText,
            verseRangeReferences: hadith.VerseReferences,
            references: references);
        HadithWriteRepository.Write(hadithViewModel);
    }
}
