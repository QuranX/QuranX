namespace QuranX.DocumentModel.Tests;

[Trait("Category", "Unit")]
public sealed class StringHelperTests
{
    [Fact]
    public void ToHexValues_Ascii_ReturnsUtf8HexEncoding()
    {
        Assert.Equal("414243", "ABC".ToHexValues());
    }

    [Fact]
    public void ToHexValues_Empty_ReturnsEmpty()
    {
        Assert.Equal("", "".ToHexValues());
    }

    [Fact]
    public void ToHexValues_Unicode_UsesUtf8MultiByteEncoding()
    {
        // U+0041 = "A" -> 0x41 (1 byte), U+00E9 = "é" -> 0xC3 0xA9 (2 bytes).
        Assert.Equal("41c3a9", "Aé".ToHexValues());
    }
}
