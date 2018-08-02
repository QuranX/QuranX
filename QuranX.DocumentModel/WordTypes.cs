using System.Collections.Generic;

namespace QuranX.DocumentModel
{
	public static class WordTypes
	{
		public static Dictionary<string, string> Values = new Dictionary<string, string>();

		static WordTypes()
		{
			Values["P"] = "";
			Values["DET"] = "";
			Values["N"] = "noun";
			Values["PN"] = "proper noun";
			Values["ADJ"] = "adjective";
			Values["PRON"] = "pronoun";
			Values["V"] = "verb";
			Values["CONJ"] = "conjunction";
			Values["REL"] = "relative pronoun";
			Values["NEG"] = "negative particle";
			Values["INL"] = "Quran initials";
			Values["DEM"] = "demonstrative pronoun";
			Values["REM"] = "resumption particle";
			Values["ACC"] = "accusative particle";
			Values["EQ"] = "equalization particle";
			Values["CIRC"] = "circumstantial particle";
			Values["RES"] = "restriction particle";
			Values["T"] = "time adverb";
			Values["PRO"] = "prohibition particle";
			Values["PREV"] = "preventive particle";
			Values["INC"] = "inceptive particle";
			Values["SUP"] = "supplemental particle";
			Values["AMD"] = "amendment particle";
			Values["SUB"] = "subordinating conjunction";
			Values["INTG"] = "interrogative";
			Values["LOC"] = "location adverb";
			Values["COND"] = "conditional particle";
			Values["EMPH"] = "emphatic prefix";
			Values["VOC"] = "vocative particle";
			Values["RSLT"] = "prefixed result particle";
			Values["EXL"] = "explanation particle";
			Values["EXP"] = "exceptive particle";
			Values["CAUS"] = "prefixed particle of cause";
			Values["FUT"] = "prefixed future particle";
			Values["CERT"] = "particle of certainty";
			Values["PRP"] = "prefixed particle of purpose";
			Values["ANS"] = "answer particle";
			Values["RET"] = "retraction particle";
			Values["EXH"] = "exhortation particle";
			Values["INT"] = "particle of interpretation";
			Values["IMPV"] = "prefixed imperative particle";
			Values["COM"] = "prefixed comitative particle";
			Values["SUR"] = "surprise particle";
			Values["AVR"] = "aversion particle";
			Values["IMPN"] = "imperative verbal noun"; 
		}

	}
}
