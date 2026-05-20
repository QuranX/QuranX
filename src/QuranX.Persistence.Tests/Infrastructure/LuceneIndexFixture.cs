using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using QuranX.Persistence.LuceneSupport;
using QuranX.Persistence.Services;
using System;
using LuceneDirectory = Lucene.Net.Store.Directory;

namespace QuranX.Persistence.Tests.Infrastructure;

public sealed class LuceneIndexFixture : IDisposable
{
    private readonly RAMDirectory _directory;
    private readonly QuranXAnalyzer _analyzer;
    private readonly IndexWriter _indexWriter;
    private bool _disposed;

    public ILuceneDirectoryProvider DirectoryProvider { get; }
    public ILuceneAnalyzerProvider AnalyzerProvider { get; }
    public ILuceneIndexWriterProvider IndexWriterProvider { get; }
    public ILuceneIndexSearcherProvider SearcherProvider { get; }

    public LuceneIndexFixture()
    {
        _directory = new RAMDirectory();
        _analyzer = new QuranXAnalyzer();
        var config = new IndexWriterConfig(Consts.LuceneVersion, _analyzer)
        {
            OpenMode = OpenMode.CREATE_OR_APPEND,
        };
        _indexWriter = new IndexWriter(_directory, config);
        _indexWriter.Commit();

        DirectoryProvider = new StubDirectoryProvider(_directory);
        AnalyzerProvider = new StubAnalyzerProvider(_analyzer);
        IndexWriterProvider = new StubIndexWriterProvider(_indexWriter);
        SearcherProvider = new RefreshingSearcherProvider(_directory);
    }

    public void Reseed(Action<IndexWriter> seed)
    {
        _indexWriter.DeleteAll();
        seed?.Invoke(_indexWriter);
        _indexWriter.Commit();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;

        (SearcherProvider as IDisposable)?.Dispose();
        _indexWriter.Dispose();
        _directory.Dispose();
        _analyzer.Dispose();
    }

    private sealed class StubDirectoryProvider : ILuceneDirectoryProvider
    {
        private readonly LuceneDirectory _directory;
        public StubDirectoryProvider(LuceneDirectory directory) => _directory = directory;
        public LuceneDirectory GetDirectory() => _directory;
    }

    private sealed class StubAnalyzerProvider : ILuceneAnalyzerProvider
    {
        private readonly Analyzer _analyzer;
        public StubAnalyzerProvider(Analyzer analyzer) => _analyzer = analyzer;
        public Analyzer GetAnalyzer() => _analyzer;
    }

    private sealed class StubIndexWriterProvider : ILuceneIndexWriterProvider
    {
        private readonly IndexWriter _indexWriter;
        public StubIndexWriterProvider(IndexWriter indexWriter) => _indexWriter = indexWriter;
        public IndexWriter GetIndexWriter() => _indexWriter;
    }

    private sealed class RefreshingSearcherProvider : ILuceneIndexSearcherProvider, IDisposable
    {
        private readonly LuceneDirectory _directory;
        private DirectoryReader? _reader;
        private IndexSearcher? _searcher;

        public RefreshingSearcherProvider(LuceneDirectory directory)
        {
            _directory = directory;
        }

        public IndexSearcher GetIndexSearcher()
        {
            if (_reader is null)
            {
                _reader = DirectoryReader.Open(_directory);
                _searcher = new IndexSearcher(_reader);
                return _searcher;
            }

            DirectoryReader? updated = DirectoryReader.OpenIfChanged(_reader);
            if (updated is not null)
            {
                _reader.Dispose();
                _reader = updated;
                _searcher = new IndexSearcher(_reader);
            }
            return _searcher!;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }
}
