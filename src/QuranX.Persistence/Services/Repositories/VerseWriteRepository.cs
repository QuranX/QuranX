using Lucene.Net.Documents;
using Lucene.Net.Index;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuranX.Persistence.Services.Repositories;

public interface IVerseWriteRepository
{
    void Write(Verse verse);
}

public class VerseWriteRepository : IVerseWriteRepository
{
    private readonly ILuceneIndexWriterProvider IndexWriterProvider;

    public VerseWriteRepository(ILuceneIndexWriterProvider indexWriterProvider)
    {
        IndexWriterProvider = indexWriterProvider;
    }

    public void Write(Verse verse)
    {
        if (verse is null)
            throw new ArgumentNullException(nameof(verse));

        float boostValue = DocumentWeights.Weights["Quran"];
        var document = new Document();
        IEnumerable<string> searchableText = verse.VerseTexts
            .Where(x => string.Compare(x.TranslatorCode, "Transliteration", true) != 0)
            .Select(x => x.Text);
        document
            .StoreAndIndex(verse, x => x.Id)
            .StoreAndIndex(verse, x => x.ChapterNumber)
            .StoreAndIndex(verse, x => x.VerseNumber)
            .AddSearchableText(searchableText, boostValue)
            .AddObject(verse);

        IndexWriter indexWriter = IndexWriterProvider.GetIndexWriter();
        indexWriter.AddDocument(document);
    }
}
