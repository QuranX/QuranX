using NSubstitute;
using QuranX.Persistence.Models;
using QuranX.Persistence.Services.Repositories;
using QuranX.Shared.Models;
using QuranX.Web.Factories;
using QuranX.Web.Views.Shared;

namespace QuranX.Web.Tests.Factories;

[Trait("Category", "Unit")]
public sealed class HadithViewModelFactoryTests
{
    private static HadithCollection BuildCollection() => new(
        code: "Bukhari",
        name: "Sahih al-Bukhari",
        referenceDefinitions:
        [
            new HadithReferenceDefinition(
                collectionCode: "Bukhari",
                code: "USC",
                name: "USC-MSA",
                valuePrefix: "",
                partNames: ["Volume", "Book", "Number"],
                isPrimary: true),
        ],
        hadithCount: 1);

    private static Hadith BuildHadith() => new(
        collectionCode: "Bukhari",
        arabicText: ["arabic"],
        englishText: ["english"],
        verseRangeReferences: System.Array.Empty<VerseRangeReference>(),
        references:
        [
            new HadithReference(
                collectionCode: "Bukhari",
                referenceCode: "USC",
                referenceValue1: 1,
                referenceValue2: 2,
                referenceValue3: 3,
                suffix: "",
                primaryReferencePath: "Bukhari/USC/1/2/3"),
        ],
        primaryReferenceCode: "USC",
        primaryReferencePath: "Bukhari/USC/1/2/3");

    [Fact]
    public void Create_BuildsViewModelWithCollectionName()
    {
        var collection = BuildCollection();
        var repo = Substitute.For<IHadithCollectionRepository>();
        string collectionCode = "Bukhari";
        repo.Get(ref collectionCode).Returns(collection);

        var factory = new HadithViewModelFactory(repo);

        IEnumerable<HadithViewModel> result = factory.Create([BuildHadith()]);

        HadithViewModel viewModel = result.Single();
        Assert.Equal("Sahih al-Bukhari", viewModel.CollectionName);
        Assert.Single(viewModel.References);
    }

    [Fact]
    public void Create_BuildsReferenceWithCorrectFields()
    {
        var collection = BuildCollection();
        var repo = Substitute.For<IHadithCollectionRepository>();
        string collectionCode = "Bukhari";
        repo.Get(ref collectionCode).Returns(collection);

        var factory = new HadithViewModelFactory(repo);

        HadithViewModel viewModel = factory.Create([BuildHadith()]).Single();
        HadithReferenceViewModel reference = viewModel.References[0];

        Assert.Equal("Bukhari", reference.CollectionCode);
        Assert.Equal("Sahih al-Bukhari", reference.CollectionName);
        Assert.Equal("USC", reference.IndexCode);
        Assert.Equal("USC-MSA", reference.IndexName);
    }

    [Fact]
    public void Create_EmptyInput_ReturnsEmpty()
    {
        var repo = Substitute.For<IHadithCollectionRepository>();
        var factory = new HadithViewModelFactory(repo);

        Assert.Empty(factory.Create([]));
    }
}
