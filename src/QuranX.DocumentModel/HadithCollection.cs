using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuranX.Shared.Models;

namespace QuranX.DocumentModel
{
	public class HadithCollection
	{
		readonly Dictionary<VerseReference, List<Hadith>> HadithsByVerse;
		readonly List<Hadith> _Hadiths;
		public readonly string Code;
		public readonly string Name;
		public readonly string Copyright;
		public readonly HadithReferenceDefinition[] ReferenceDefinitions;
		HadithReferenceDefinition _PrimaryReferenceDefinition;
		Dictionary<string, HadithReferenceDefinition> ReferenceDefinitionsByCode;

		public HadithCollection(
			string code,
			string name,
			string copyright,
			IEnumerable<HadithReferenceDefinition> referenceDefinitions)
		{
			referenceDefinitions = referenceDefinitions ?? new HadithReferenceDefinition[0];
			this.Code = code;
			this.Name = name;
			this.Copyright = copyright;
			this.ReferenceDefinitions = referenceDefinitions.ToArray();
			_Hadiths = new List<Hadith>();
			HadithsByVerse = new Dictionary<VerseReference, List<Hadith>>();
			ReferenceDefinitionsByCode = referenceDefinitions.ToDictionary(x => x.Code, StringComparer.InvariantCultureIgnoreCase);
		}

		public HadithReferenceDefinition GetReferenceDefinition(string code)
		{
			HadithReferenceDefinition result;
			if (ReferenceDefinitionsByCode.TryGetValue(code, out result))
				return result;
			return null;
		}

		public bool IsReferenceValid(HadithReference reference)
		{
			if (reference == null)
				throw new ArgumentNullException(nameof(reference));
			var referenceDefinition = GetReferenceDefinition(reference.Code);
			bool result = reference.Count(x => !string.IsNullOrEmpty(x)) == referenceDefinition.PartNames.Length;
			return result;
		}

		public HadithReferenceDefinition PrimaryReferenceDefinition
		{
			get
			{
				if (_PrimaryReferenceDefinition == null)
				{
					var result = ReferenceDefinitions.Where(x => x.IsPrimary).ToArray();
					if (result.Length < 1)
						throw new InvalidOperationException("Collection " + Code + " has no primary reference definition");
					else if (result.Length > 1)
						throw new InvalidOperationException("Collection " + Code + " has more than one primary reference definition");
					_PrimaryReferenceDefinition = result[0];
				}
				return _PrimaryReferenceDefinition;
			}
		}

		public IEnumerable<Hadith> Hadiths
		{
			get
			{
				return _Hadiths;
			}
		}

		public void AddHadith(Hadith hadith)
		{
			_Hadiths.Add(hadith);
			AddHadithToIndividualVerses(hadith);
		}

		public IEnumerable<Hadith> GetHadithsForVerse(
				int chapterIndex,
				int verseIndex)
		{
			List<Hadith> result;
			var verseReference = new VerseReference(
					chapter: chapterIndex,
					verse: verseIndex
				);
			if (!HadithsByVerse.TryGetValue(verseReference, out result))
				return new List<Hadith>();
			return result.OrderBy(x => x.PrimaryReference);
		}

		void AddHadithToIndividualVerses(Hadith hadith)
		{
			foreach (var verseRangeReference in hadith.VerseReferences)
			{
				for (int verseIndex = verseRangeReference.FirstVerse;
						verseIndex <= verseRangeReference.LastVerse;
						verseIndex++
					)
				{
					List<Hadith> hadiths;
					var verseReference = new VerseReference(
							chapter: verseRangeReference.Chapter,
							verse: verseIndex
						);
					if (!HadithsByVerse.TryGetValue(verseReference, out hadiths))
					{
						hadiths = new List<Hadith>();
						HadithsByVerse[verseReference] = hadiths;
					}
					hadiths.Add(hadith);
				}
			}
		}

	}
}
