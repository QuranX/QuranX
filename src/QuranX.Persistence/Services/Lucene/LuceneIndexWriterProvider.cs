using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Util;
using System;

namespace QuranX.Persistence.Services.Lucene;

public interface ILuceneIndexWriterProvider
{
    IndexWriter GetIndexWriter();
}

public class LuceneIndexWriterProvider : ILuceneIndexWriterProvider, IDisposable
{
    private readonly ILuceneDirectoryProvider _directoryProvider;
    private readonly Analyzer _analyzer;
    private readonly Lazy<IndexWriter> _indexWriter;

    public LuceneIndexWriterProvider(
        ILuceneDirectoryProvider luceneDirectoryProvider,
        Analyzer analyzer)
    {
        _directoryProvider = luceneDirectoryProvider;
        _analyzer = analyzer;
        _indexWriter = new Lazy<IndexWriter>(() =>
        {
            var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer)
            {
                OpenMode = OpenMode.CREATE_OR_APPEND
            };
            return new IndexWriter(_directoryProvider.GetDirectory(), indexWriterConfig);
        });
    }

    public IndexWriter GetIndexWriter() => _indexWriter.Value;

    public void Dispose()
    {
        if (_indexWriter.IsValueCreated)
        {
            _indexWriter.Value.Dispose();
        }
        _directoryProvider.GetDirectory().Dispose();
    }
}
