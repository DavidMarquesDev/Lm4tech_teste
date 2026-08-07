using System.Globalization;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Application.Messaging;
using ProductChallenge.Domain;

namespace ProductChallenge.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IServiceBus<ProductChangedNotification> _bus;

    public ProductService(IProductRepository repository, IServiceBus<ProductChangedNotification> bus)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _bus = bus ?? throw new ArgumentNullException(nameof(bus));
    }

    public Task<PagedResult<Product>> ListAsync(string searchTerm, int pageNumber, int pageSize)
    {
        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "O tamanho da página deve ser positivo.");
        }

        return _repository.GetPageAsync(searchTerm ?? string.Empty, Math.Max(1, pageNumber), pageSize);
    }

    public async Task CreateAsync(ProductDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var product = Product.Create(
            draft.Name, draft.Description, draft.Price, draft.Category, draft.StockQuantity);

        await _repository.AddAsync(product);
        await Publish(product, ProductChange.Created, DescribeCreation(product));
    }

    public async Task UpdateAsync(int id, ProductDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var product = await _repository.GetByIdAsync(id);

        // SetDetails altera a entidade no lugar, então os valores anteriores são copiados antes.
        var before = Snapshot(product);

        product.SetDetails(
            draft.Name, draft.Description, draft.Price, draft.Category, draft.StockQuantity);

        await _repository.UpdateAsync(product);
        await Publish(product, ProductChange.Updated, DescribeUpdate(before, product));
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        await _repository.DeleteAsync(id);
        await Publish(product, ProductChange.Deleted, []);
    }

    private Task Publish(Product product, ProductChange change, IReadOnlyList<FieldChange> changes) =>
        _bus.PublishAsync(new ProductChangedNotification(product.Id, product.Name, change, changes));

    private static ProductValues Snapshot(Product product) => new(
        product.Name, product.Description, product.Price, product.Category, product.StockQuantity);

    private static IReadOnlyList<FieldChange> DescribeCreation(Product product)
    {
        var changes = new List<FieldChange>
        {
            new("Preço", null, Money(product.Price)),
            new("Categoria", null, ProductCategoryCatalog.LabelFor(product.Category)),
            new("Estoque", null, Number(product.StockQuantity))
        };

        if (product.Description is not null)
        {
            changes.Add(new FieldChange("Descrição", null, product.Description));
        }

        return changes;
    }

    private static IReadOnlyList<FieldChange> DescribeUpdate(ProductValues before, Product after)
    {
        var changes = new List<FieldChange>();

        Compare("Nome", before.Name, after.Name);
        Compare("Descrição", before.Description, after.Description);
        Compare("Preço", Money(before.Price), Money(after.Price));
        Compare("Categoria",
            ProductCategoryCatalog.LabelFor(before.Category),
            ProductCategoryCatalog.LabelFor(after.Category));
        Compare("Estoque", Number(before.StockQuantity), Number(after.StockQuantity));

        return changes;

        void Compare(string field, string? previous, string? current)
        {
            if (!string.Equals(previous, current, StringComparison.Ordinal))
            {
                changes.Add(new FieldChange(field, previous ?? "(vazio)", current ?? "(vazio)"));
            }
        }
    }

    private static string Money(decimal value) => value.ToString("N2", CultureInfo.CurrentCulture);

    private static string Number(int value) => value.ToString(CultureInfo.CurrentCulture);

    private sealed record ProductValues(
        string Name, string? Description, decimal Price, ProductCategory Category, int StockQuantity);
}
