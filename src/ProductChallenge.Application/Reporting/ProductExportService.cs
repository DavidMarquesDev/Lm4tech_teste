using Microsoft.Extensions.Logging;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Domain;

namespace ProductChallenge.Application.Reporting;

public sealed class ProductExportService : IProductExportService
{
    private readonly IProductRepository _repository;
    private readonly ICsvReportWriter _writer;
    private readonly ILogger<ProductExportService> _logger;

    public ProductExportService(
        IProductRepository repository,
        ICsvReportWriter writer,
        ILogger<ProductExportService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IReadOnlyList<ExportField> GetAvailableFields() => ExportFieldCatalog.For<Product>();

    public async Task<int> ExportAsync(
        IReadOnlyList<string> fieldNames, string searchTerm, Stream destination)
    {
        ArgumentNullException.ThrowIfNull(fieldNames);
        ArgumentNullException.ThrowIfNull(destination);

        // A ordem escolhida pelo usuário é preservada: a seleção é percorrida, não o catálogo.
        var available = GetAvailableFields();
        var selected = fieldNames
            .Select(name => available.FirstOrDefault(field => field.PropertyName == name))
            .OfType<ExportField>()
            .ToList();

        if (selected.Count == 0)
        {
            throw new InvalidOperationException("Selecione ao menos uma coluna para exportar.");
        }

        using var scope = _logger.BeginScope("Exportação de produtos");

        var products = await _repository.ListAsync(searchTerm ?? string.Empty);
        await _writer.WriteAsync(products, selected, destination);

        _logger.LogInformation(
            "Exportadas {RowCount} linha(s) em {ColumnCount} coluna(s).", products.Count, selected.Count);

        return products.Count;
    }
}
