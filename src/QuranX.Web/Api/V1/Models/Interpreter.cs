#nullable enable
namespace QuranX.Web.Api.V1.Models;

public class Interpreter
{
    public string Code { get; }
    public string Name { get; }

    public Interpreter(string code, string name)
    {
        System.ArgumentException.ThrowIfNullOrWhiteSpace(code);
        System.ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Code = code;
        Name = name;
    }
}
