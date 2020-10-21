using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.AR;
using QuranX.Persistence.LuceneSupport;

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
			Analyzer = new QuranXAnalyzer();
		}

		public Analyzer GetAnalyzer()
		{
			return Analyzer;
		}
	}
}
