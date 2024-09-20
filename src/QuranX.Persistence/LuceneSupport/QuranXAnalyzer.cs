using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Ar;
using Lucene.Net.Analysis.Core;
using QuranX.Persistence.Services;

namespace QuranX.Persistence.LuceneSupport
{
	public class QuranXAnalyzer : Analyzer
	{
		protected override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
		{
			Tokenizer tokenizer = new QuranXLetterTokenizer(reader);
			TokenStream tokenStream = tokenizer;
			tokenStream = new LowerCaseFilter(Consts.LuceneVersion, tokenStream);
			tokenStream = new ArabicNormalizationFilter(tokenStream);
			tokenStream = new ArabicStemFilter(tokenStream);
			return new TokenStreamComponents(tokenizer, tokenStream);
		}
	}
}
