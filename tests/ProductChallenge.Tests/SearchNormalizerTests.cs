using ProductChallenge.Domain;

namespace ProductChallenge.Tests;

public class SearchNormalizerTests
{
    [Theory]
    [InlineData("Eletrônico", "eletronico")]
    [InlineData("Açúcar", "acucar")]
    [InlineData("Não", "nao")]
    [InlineData("CAFÉ", "cafe")]
    [InlineData("Vestuário", "vestuario")]
    [InlineData("Ãêîõü", "aeiou")]
    public void Normalize_RemovesAccentsAndLowercases(string input, string expected)
    {
        Assert.Equal(expected, SearchNormalizer.Normalize(input));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_WithMissingText_ReturnsEmpty(string? input)
    {
        Assert.Equal(string.Empty, SearchNormalizer.Normalize(input));
    }

    [Fact]
    public void Normalize_TrimsSurroundingWhitespace()
    {
        Assert.Equal("teclado", SearchNormalizer.Normalize("  Teclado  "));
    }

    [Fact]
    public void Normalize_PreservesDigitsAndPunctuation()
    {
        Assert.Equal("cabo usb-c 2m, 60w", SearchNormalizer.Normalize("Cabo USB-C 2m, 60W"));
    }

    [Fact]
    public void Normalize_IsIdempotent()
    {
        var once = SearchNormalizer.Normalize("Refrigeração");

        Assert.Equal(once, SearchNormalizer.Normalize(once));
    }
}
