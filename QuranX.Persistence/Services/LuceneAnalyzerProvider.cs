using Lucene.Net.Analysis;
using Lucene.Net.Analysis.AR;

namespace QuranX.Persistence.Services
{
	public interface ILuceneAnalyzerProvider
	{
		Analyzer GetAnalyzer();
	}

	public class LuceneAnalyzerProvider : ILuceneAnalyzerProvider
	{
		private readonly Analyzer Analyzer;

		public LuceneAnalyzerProvider()
		{
			// Do not use stop-words. We use numeric values in indexes which
			// might look like a stop word such as "A" or "a".
			Analyzer = new ArabicAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
		}

		public Analyzer GetAnalyzer()
		{
			return Analyzer;
		}
	}
}
