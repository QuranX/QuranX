#nullable enable
namespace QuranX.Web.Api.V1.Models;

public class Commentator
{
    public string Code { get; }
    public string Description { get; }

    public Commentator(string code, string description)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(code);
        System.ArgumentException.ThrowIfNullOrWhiteSpace(description);

        Code = code;
        Description = description;
    }
}