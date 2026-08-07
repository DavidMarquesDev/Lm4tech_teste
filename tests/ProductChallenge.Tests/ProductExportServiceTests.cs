using System.Globalization;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using ProductChallenge.Application;
using ProductChallenge.Application.Messaging;
using ProductChallenge.Application.Reporting;
using ProductChallenge.Application.Services;
using ProductChallenge.Domain;
using ProductChallenge.Infrastructure.Messaging;
using ProductChallenge.Infrastructure.Reporting;
using ProductChallenge.Infrastructure.Repositories;

namespace ProductChallenge.Tests;

public class ProductExportServiceTests : IDisposable
{
    private readonly SqliteInMemoryDatabase _database = new();
    private readonly ProductExportService _service;
    private readonly ProductService _products;

    public ProductExportServiceTests()
    {
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

        var repository = new ProductRepository(_database);
        var bus = new InProcessServiceBus<ProductChangedNotification>(
            NullLogger<InProcessServiceBus<ProductChangedNotification>>.Instance);

        _products = new ProductService(repository, bus);
        _service = new ProductExportService(
            repository,
            new CsvReportWriter(NullLogger<CsvReportWriter>.Instance),
            NullLogger<ProductExportService>.Instance);
    }

    public void Dispose() => _database.Dispose();

    private Task AddAsync(string name, ProductCategory category = ProductCategory.Toys) =>
        _products.CreateAsync(new ProductDraft(name, $"Ficha de {name}", 10m, category, 1));

    private async Task<string> ExportAsync(IReadOnlyList<string> fields, string term = "")
    {
        using var stream = new MemoryStream();
        await _service.ExportAsync(fields, term, stream);

        // Descarta o BOM para as assertivas compararem só o conteúdo.
        return Encoding.UTF8.GetString(stream.ToArray()).TrimStart('﻿');
    }

    [Fact]
    public void GetAvailableFields_MatchesTheCatalog()
    {
        Assert.Equal(ExportFieldCatalog.For<Product>(), _service.GetAvailableFields());
    }

    [Fact]
    public async Task ExportAsync_WithoutFields_Throws()
    {
        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExportAsync([], string.Empty, stream));
    }

    [Fact]
    public async Task ExportAsync_WithUnknownFieldNames_Throws()
    {
        using var stream = new MemoryStream();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExportAsync(["NaoExiste"], string.Empty, stream));
    }

    [Fact]
    public async Task ExportAsync_WithoutFilter_WritesEveryProduct()
    {
        await AddAsync("Monitor");
        await AddAsync("Teclado");
        await AddAsync("Mouse");

        using var stream = new MemoryStream();
        var rowCount = await _service.ExportAsync(["Name"], string.Empty, stream);

        Assert.Equal(3, rowCount);
    }

    [Fact]
    public async Task ExportAsync_RespectsTheFilterInUse()
    {
        await AddAsync("Monitor Ultrawide");
        await AddAsync("Monitor Curvo");
        await AddAsync("Teclado mecânico");

        using var stream = new MemoryStream();
        var rowCount = await _service.ExportAsync(["Name"], "monitor", stream);

        Assert.Equal(2, rowCount);
    }

    [Fact]
    public async Task ExportAsync_FilterIgnoresAccents()
    {
        await AddAsync("Teclado mecânico");

        using var stream = new MemoryStream();
        var rowCount = await _service.ExportAsync(["Name"], "mecanico", stream);

        Assert.Equal(1, rowCount);
    }

    [Fact]
    public async Task ExportAsync_WritesOnlyTheSelectedColumns()
    {
        await AddAsync("Monitor");

        var content = await ExportAsync(["Name"]);

        Assert.Contains("Produto", content);
        Assert.DoesNotContain("Estoque", content);
    }

    [Fact]
    public async Task ExportAsync_KeepsTheOrderChosenByTheUser()
    {
        await AddAsync("Monitor");

        var content = await ExportAsync(["StockQuantity", "Name"]);
        var header = content.Split("\r\n")[0];

        Assert.StartsWith("Estoque", header, StringComparison.Ordinal);
        Assert.EndsWith("Produto", header, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExportAsync_TranslatesTheCategoryFromTheEnum()
    {
        await AddAsync("Monitor", ProductCategory.Electronics);

        var content = await ExportAsync(["Category"]);

        Assert.Contains(nameof(ProductCategory.Electronics), content);
    }
}
