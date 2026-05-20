namespace QuranX.Web.E2ETests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class QuranXCollection : ICollectionFixture<WebHostFixture>
{
    public const string Name = "QuranX";
}
