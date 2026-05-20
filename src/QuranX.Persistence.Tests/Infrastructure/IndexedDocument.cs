using Lucene.Net.Index;
using QuranX.Persistence.Services;

namespace QuranX.Persistence.Tests.Infrastructure;

internal sealed class StubWriterProvider : ILuceneIndexWriterProvider
{
    private readonly IndexWriter _writer;
    public StubWriterProvider(IndexWriter writer) => _writer = writer;
    public IndexWriter GetIndexWriter() => _writer;
}
