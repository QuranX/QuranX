using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class Hadith
	{
        public readonly HadithCollection Collection;
        public readonly HadithReference PrimaryReference;
        public readonly HadithReference[] References;
        public readonly string[] ArabicText;
		public readonly string[] EnglishText;
		public readonly VerseRangeReference[] VerseReferences;
        Dictionary<string, HadithReference> ReferencesByCode;

		public Hadith(
            HadithCollection collection,
            IEnumerable<HadithReference> references,
            IEnumerable<string> arabicText,
			IEnumerable<string> englishText,
			IEnumerable<VerseRangeReference> verseReferences)
		{
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (references == null || !references.Any())
                throw new ArgumentNullException(nameof(references));
            if (arabicText == null)
                throw new ArgumentNullException(nameof(arabicText));
            if (englishText == null)
                throw new ArgumentNullException(nameof(englishText));
            if (verseReferences == null)
                throw new ArgumentNullException(nameof(verseReferences));

            this.Collection = collection;
            this.References = references.ToArray();
            this.ArabicText = arabicText.ToArray();
            this.EnglishText = englishText.ToArray();
			this.VerseReferences = verseReferences.Distinct().OrderBy(x => x).ToArray();
            ReferencesByCode = references.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);
            PrimaryReference = GetReference(collection.PrimaryReferenceDefinition.Code) ?? References.First();
		}

        public HadithReference GetReference(string code)
        {
            HadithReference result;
            if (ReferencesByCode.TryGetValue(code, out result))
                return result;
            return null;
        }
	}
}
