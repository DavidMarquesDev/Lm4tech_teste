using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using ProductChallenge.Application.Reporting;
using ProductChallenge.Domain;
using ProductChallenge.Infrastructure.Reporting;

namespace ProductChallenge.Tests;

public class CsvReportWriterTests
{
    private readonly CsvReportWriter _writer = new(NullLogger<CsvReportWriter>.Instance);

    public CsvReportWriterTests() => CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

    private static string Separator => CultureInfo.CurrentCulture.TextInfo.ListSeparator;

    private static IReadOnlyList<ExportField> Fields(params string[] names) =>
        ExportFieldCatalog.For<Product>()
            .Where(field => names.Contains(field.PropertyName))
            .OrderBy(field => Array.IndexOf(names, field.PropertyName))
            .ToList();

    private async Task<string> WriteAsync(IEnumerable<Product> products, params string[] names)
    {
        using var stream = new MemoryStream();
        await _writer.WriteAsync(products, Fields(names), stream);

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    [Fact]
    public async Task WriteAsync_WithoutFields_Throws()
    {
        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<ArgumentException>(
            () => _writer.WriteAsync(Array.Empty<Product>(), [], stream));
    }

    [Fact]
    public async Task WriteAsync_StartsWithTheByteOrderMark()
    {
        using var stream = new MemoryStream();
        await _writer.WriteAsync(Array.Empty<Product>(), Fields("Name"), stream);

        var bytes = stream.ToArray();

        // Sem o BOM o Excel abre "Preço" como "PreÃ§o".
        Assert.Equal([0xEF, 0xBB, 0xBF], bytes.Take(3));
    }

    [Fact]
    public async Task WriteAsync_UsesTheHeadersDeclaredInTheAttributes()
    {
        var content = await WriteAsync([], "Name", "Price");

        Assert.Contains($"Produto{Separator}Preço", content);
    }

    [Fact]
    public async Task WriteAsync_KeepsTheOrderChosenByTheCaller()
    {
        var content = await WriteAsync([], "Price", "Name");

        Assert.Contains($"Preço{Separator}Produto", content);
    }

    [Fact]
    public async Task WriteAsync_QuotesValuesContainingTheSeparator()
    {
        // O separador vem da cultura, então o valor de teste é montado a partir dele.
        var name = $"Cabo USB-C{Separator} 2m";
        var product = Product.Create(name, null, 39.90m, ProductCategory.Electronics, 1);

        var content = await WriteAsync([product], "Name");

        Assert.Contains($"\"{name}\"", content);
    }

    [Fact]
    public async Task WriteAsync_DoublesEmbeddedQuotes()
    {
        var product = Product.Create("Monitor \"widescreen\"", null, 10m, ProductCategory.Electronics, 1);

        var content = await WriteAsync([product], "Name");

        Assert.Contains("\"Monitor \"\"widescreen\"\"\"", content);
    }

    [Fact]
    public async Task WriteAsync_QuotesValuesContainingLineBreaks()
    {
        var product = Product.Create("Monitor", "Linha 1\r\nLinha 2", 10m, ProductCategory.Electronics, 1);

        var content = await WriteAsync([product], "Description");

        Assert.Contains("\"Linha 1\r\nLinha 2\"", content);
    }

    [Fact]
    public async Task WriteAsync_FormatsNumbersUsingTheCurrentCulture()
    {
        var product = Product.Create("Monitor", null, 1899.50m, ProductCategory.Electronics, 1);

        var content = await WriteAsync([product], "Price");

        Assert.Contains("1.899,50", content);
    }

    [Fact]
    public async Task WriteAsync_LeavesAbsentValuesEmpty()
    {
        var product = Product.Create("Monitor", null, 10m, ProductCategory.Electronics, 1);

        var content = await WriteAsync([product], "Name", "Description");

        Assert.Contains($"Monitor{Separator}\r\n", content);
    }

    [Fact]
    public async Task WriteAsync_EmitsOneLinePerRowPlusTheHeader()
    {
        var products = Enumerable.Range(1, 5)
            .Select(index => Product.Create($"Produto {index}", null, 10m, ProductCategory.Toys, 1))
            .ToList();

        var content = await WriteAsync(products, "Name");
        var lines = content.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        Assert.Equal(6, lines.Length);
    }

    [Fact]
    public async Task WriteAsync_LeavesTheStreamOpenForTheCaller()
    {
        using var stream = new MemoryStream();
        await _writer.WriteAsync(Array.Empty<Product>(), Fields("Name"), stream);

        Assert.True(stream.CanWrite);
    }
}
