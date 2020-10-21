using System;
using System.Collections.Generic;

namespace QuranX.Persistence
{
	public static class DocumentWeights
	{
		private const float Upper = 8f;
		public static readonly IReadOnlyDictionary<string, float> Weights =
			new Dictionary<string, float>(StringComparer.InvariantCultureIgnoreCase) {
				["Quran"] = Upper - 1f,
				["Hadith-Bukhari"] = Upper - 2f,
				["Hadith-Muslim"] = Upper - 2f,
				["Hadith-Nasai"] = Upper - 3f,
				["Hadith-AbuDawud"] = Upper - 3f,
				["Hadith-IbnMajah"] = Upper - 3f,
				["Hadith-Malik"] = Upper - 3f,
				["Hadith-Tirmidhi"] = Upper - 3f,
				["Hadith-Adab"] = Upper - 4f,
				["Hadith-Maram"] = Upper - 4f,
				["Hadith-Saliheen"] = Upper - 4f,
				["Hadith-Shamail"] = Upper - 4f,
				["Commentary-Jalal"] = Upper - 5f,
				["Commentary-Kathir"] = Upper - 5f,
				["Commentary-Abbas"] = Upper - 6f,
				["Commentary-Asrar"] = Upper - 6f,
				["Commentary-Kashani"] = Upper - 6f,
				["Commentary-Qushairi"] = Upper - 6f,
				["Commentary-Tustari"] = Upper - 6f,
				["Commentary-Wahidi"] = Upper - 6f,
				["Commentary-Maududi"] = Upper - 7f
			};
	}
}
