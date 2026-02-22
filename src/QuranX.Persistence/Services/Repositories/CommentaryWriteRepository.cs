using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using System;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories;

public interface ICommentaryWriteRepository
{
    void Write(Commentary commentary);
}

public class CommentaryWriteRepository : ICommentaryWriteRepository
{
    private readonly ILuceneIndexWriterProvider IndexWriterProvider;

    public CommentaryWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
    {
        IndexWriterProvider = indexWriterProvider;
    }

    public void Write(Commentary commentary)
    {
        if (commentary is null)
            throw new ArgumentNullException(nameof(commentary));

        float boostValue = DocumentWeights.Weights["Commentary-" + commentary.CommentatorCode];
        var document = new Document();
        document.StoreAndIndex(commentary, x => x.CommentatorCode);
        document.StoreAndIndex(commentary, x => x.ChapterNumber);
        document.StoreAndIndex(commentary, x => x.FirstVerseNumber);
        document.StoreAndIndex(commentary, x => x.LastVerseNumber);
        document.AddSearchableText(commentary.Text.Select(x => x.Text), boostValue);
        document.AddObject(commentary);

        IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
        indexWriter.AddDocument(document);
    }
}

