using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace QuranX.DocumentModel;

public class HadithComparer : IComparer<Hadith>
{
    public readonly ImmutableArray<string> CollectionCodesInPriorityOrder;

    public HadithComparer(IEnumerable<HadithReferenceDefinition> hadithReferenceDefinitions)
    {
        ArgumentNullException.ThrowIfNull(hadithReferenceDefinitions);
        HadithReferenceDefinition primaryReference = hadithReferenceDefinitions.Single(x => x.IsPrimary);
        HadithReferenceDefinition[] sortedReferenceDefinitions = [
                primaryReference,
                ..hadithReferenceDefinitions.Where(x => !x.IsPrimary)
            ];
        CollectionCodesInPriorityOrder =
            sortedReferenceDefinitions
            .Select(x => x.Code)
            .ToImmutableArray();
    }

    public int Compare(Hadith first, Hadith second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        var firstHadithReferences = first.References.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
        var secondHadithReferences = second.References.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

        // Return based on first matching references
        foreach(string definitionCode in CollectionCodesInPriorityOrder)
        {
            bool firstHasReference = firstHadithReferences.TryGetValue(definitionCode, out HadithReference firstReference);
            bool secondHasReference = secondHadithReferences.TryGetValue(definitionCode, out HadithReference secondReference);
            if (firstHasReference && secondHasReference)
                return firstReference.CompareTo(secondReference);
        }

        // If two don't have references of the same type, then return the one that has the first reference in the proprity order
        foreach (string definitionCode in CollectionCodesInPriorityOrder)
        {
            if (firstHadithReferences.ContainsKey(definitionCode)) return -1;
            if (firstHadithReferences.ContainsKey(definitionCode)) return 1;
        }

        // Just return the same
        return 0;
    }
}
