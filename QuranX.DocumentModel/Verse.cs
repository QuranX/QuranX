using System.Collections.Generic;
using System.Linq;

namespace QuranX.DocumentModel
{
	public class Verse
	{
		readonly HashSet<VerseTranslation> _Translations;
		public readonly int Index;
		public readonly string ArabicText;
		
		public Verse(int index, string arabicText)
		{
			this.Index = index;
			this._Translations = new HashSet<VerseTranslation>();
			this.ArabicText = arabicText;
		}

		public IEnumerable<VerseTranslation> Translations
		{
			get 
			{
				return _Translations
					.OrderBy(x => x.TranslatorName);
			}
		}

		public void AddTranslation(VerseTranslation translation)
		{
			_Translations.Add(translation);
		}
	}
}
