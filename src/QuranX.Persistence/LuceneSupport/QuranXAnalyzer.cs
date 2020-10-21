using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.AR;

namespace QuranX.Persistence.LuceneSupport
{
	public class QuranXAnalyzer : Analyzer
	{
		public override TokenStream TokenStream(string fieldName, TextReader reader)
		{
			TokenStream @in = new QuranXLetterTokenizer(reader);
			@in = new LowerCaseFilter(@in);
			return new ArabicStemFilter(new ArabicNormalizationFilter(@in));
		}
	}
}
