using Lucene.Net.Documents;
using NSubstitute;
using QuranX.Persistence.Extensions;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Web.Services;
using QuranX.Web.Views.Search;

namespace QuranX.Web.Tests.Factories;

[Trait("Category", "Unit")]
public sealed class SearchResultWithLinkFactoryTests
{
    private static Document BuildVerseDocument(int chapter, int verse)
    {
        var document = new Document();
        document.StoreAndIndex(new Verse(
                chapterNumber: chapter,
                verseNumber: verse,
                rootWordCount: 0,
                hadithCount: 0,
                commentaryCount: 0,
                verseTexts: []),
            x => x.ChapterNumber);
        document.StoreAndIndex(new Verse(
                chapterNumber: chapter,
                verseNumber: verse,
                rootWordCount: 0,
                hadithCount: 0,
                commentaryCount: 0,
                verseTexts: []),
            x => x.VerseNumber);
        return document;
    }

    private static Document BuildCommentaryDocument(string code, int chapter, int verse)
    {
        var document = new Document();
        var commentary = new Commentary(
            commentatorCode: code,
            chapterNumber: chapter,
            firstVerseNumber: verse,
            lastVerseNumber: verse,
            text: []);
        document.StoreAndIndex(commentary, x => x.CommentatorCode);
        document.StoreAndIndex(commentary, x => x.ChapterNumber);
        document.StoreAndIndex(commentary, x => x.FirstVerseNumber);
        return document;
    }

    private static Document BuildHadithDocument()
    {
        var document = new Document();
        var hadith = new Hadith(
            collectionCode: "Bukhari",
            arabicText: [],
            englishText: [],
            verseRangeReferences: [],
            references: [],
            primaryReferenceCode: "USC",
            primaryReferencePath: "Bukhari/USC/1");
        document.StoreAndIndex(hadith, x => x.CollectionCode);
        document.StoreAndIndex(hadith, x => x.PrimaryReferenceCode);
        document.StoreAndIndex(hadith, x => x.PrimaryReferencePath);
        return document;
    }

    [Fact]
    public void Create_VerseResult_ReturnsQuranUrl()
    {
        var commentatorRepo = Substitute.For<ICommentatorRepository>();
        var collectionRepo = Substitute.For<IHadithCollectionRepository>();
        var mapper = new SearchResultWithLinkMapper(commentatorRepo, collectionRepo);

        var searchResult = new SearchResult(
            type: nameof(Verse),
            document: BuildVerseDocument(2, 5),
            snippets: ["snippet"]);

        SearchResultWithLink result = mapper.Create(searchResult);

        Assert.Equal("/2.5", result.Url);
        Assert.Equal("Quran 2.5", result.Caption);
    }

    [Fact]
    public void Create_CommentaryResult_ReturnsTafsirUrl()
    {
        var commentatorRepo = Substitute.For<ICommentatorRepository>();
        string code = "Kathir";
        commentatorRepo
            .TryGet(ref code, out Arg.Any<Commentator>())
            .Returns(call =>
            {
                call[1] = new Commentator("Kathir", "Ibn Kathir");
                return true;
            });
        var collectionRepo = Substitute.For<IHadithCollectionRepository>();
        var mapper = new SearchResultWithLinkMapper(commentatorRepo, collectionRepo);

        var searchResult = new SearchResult(
            type: nameof(Commentary),
            document: BuildCommentaryDocument("Kathir", 1, 3),
            snippets: []);

        SearchResultWithLink result = mapper.Create(searchResult);

        Assert.Equal("/Tafsir/Kathir/1.3", result.Url);
        Assert.Contains("Ibn Kathir", result.Caption);
    }

    [Fact]
    public void Create_HadithResult_ReturnsHadithUrl()
    {
        var commentatorRepo = Substitute.For<ICommentatorRepository>();
        var collectionRepo = Substitute.For<IHadithCollectionRepository>();
        string collectionCode = "Bukhari";
        collectionRepo.Get(ref collectionCode).Returns(new HadithCollection(
            code: "Bukhari",
            name: "Sahih al-Bukhari",
            referenceDefinitions: [
                new HadithReferenceDefinition(
                    collectionCode: "Bukhari",
                    code: "USC",
                    name: "USC",
                    valuePrefix: "",
                    partNames: ["N"],
                    isPrimary: true),
            ],
            hadithCount: 1));
        var mapper = new SearchResultWithLinkMapper(commentatorRepo, collectionRepo);

        var searchResult = new SearchResult(
            type: nameof(Hadith),
            document: BuildHadithDocument(),
            snippets: []);

        SearchResultWithLink result = mapper.Create(searchResult);

        Assert.Equal("/Hadith/Bukhari/USC/Bukhari/USC/1", result.Url);
        Assert.Contains("Sahih al-Bukhari", result.Caption);
    }

    [Fact]
    public void Create_UnknownType_Throws()
    {
        var commentatorRepo = Substitute.For<ICommentatorRepository>();
        var collectionRepo = Substitute.For<IHadithCollectionRepository>();
        var mapper = new SearchResultWithLinkMapper(commentatorRepo, collectionRepo);

        var searchResult = new SearchResult(
            type: "Unknown",
            document: new Document(),
            snippets: []);

        Assert.Throws<NotImplementedException>(() => mapper.Create(searchResult));
    }
}
