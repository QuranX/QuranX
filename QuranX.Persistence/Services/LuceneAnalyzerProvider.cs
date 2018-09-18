using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace QuranX.Persistence.Services
{
	public interface ILuceneAnalyzerProvider
	{
		Analyzer GetAnalyzer();
	}

	public class LuceneAnalyzerProvider : ILuceneAnalyzerProvider
	{
		public readonly Analyzer _analyzer;

		public LuceneAnalyzerProvider()
		{
			_analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
		}

		public Analyzer GetAnalyzer()
		{
			return _analyzer;
		}
	}
}
