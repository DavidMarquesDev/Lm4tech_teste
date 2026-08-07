using System.Globalization;
using ProductChallenge.Application;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Application.Messaging;
using ProductChallenge.Application.Services;
using ProductChallenge.Domain;

namespace ProductChallenge.Tests;

/// <summary>
/// Sem banco: o serviço só depende de <see cref="IProductRepository"/>, então um dublê basta.
/// </summary>
public class ProductServiceTests
{
    private readonly FakeProductRepository _repository = new();
    private readonly RecordingBus _bus = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
        _service = new ProductService(_repository, _bus);
    }


    private static ProductDraft Draft(string name = "Monitor", string? description = null) =>
        new(name, description, 1899.00m, ProductCategory.Electronics, 8);

    [Fact]
    public async Task ListAsync_ForwardsTermAndPagingToTheRepository()
    {
        await _service.ListAsync("monitor", 3, 30);

        Assert.Equal([nameof(IProductRepository.GetPageAsync)], _repository.Calls);
        Assert.Equal(("monitor", 3, 30), _repository.LastQuery);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task ListAsync_WithPageNumberBelowOne_AsksForTheFirstPage(int pageNumber)
    {
        await _service.ListAsync(string.Empty, pageNumber, 10);

        Assert.Equal(1, _repository.LastQuery.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ListAsync_WithNonPositivePageSize_Throws(int pageSize)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _service.ListAsync(string.Empty, 1, pageSize));
    }

    [Fact]
    public async Task ListAsync_WithNullTerm_TreatsItAsNoFilter()
    {
        await _service.ListAsync(null!, 1, 10);

        Assert.Equal(string.Empty, _repository.LastQuery.Term);
    }

    [Fact]
    public async Task CreateAsync_BuildsProductFromDraft()
    {
        await _service.CreateAsync(Draft("Monitor", "Painel IPS"));

        var product = Assert.Single(_repository.Items);
        Assert.Equal("Monitor", product.Name);
        Assert.Equal("Painel IPS", product.Description);
        Assert.Equal(1899.00m, product.Price);
        Assert.Equal(ProductCategory.Electronics, product.Category);
        Assert.Equal(8, product.StockQuantity);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDraft_DoesNotReachTheRepository()
    {
        var invalid = new ProductDraft("Monitor", null, -1m, ProductCategory.Electronics, 8);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.CreateAsync(invalid));

        Assert.Empty(_repository.Items);
    }

    [Fact]
    public async Task UpdateAsync_AppliesDraftToTheStoredProduct()
    {
        _repository.Items.Add(Product.Create("Antigo", null, 10m, ProductCategory.Toys, 1));

        await _service.UpdateAsync(0, Draft("Monitor Pro"));

        var product = Assert.Single(_repository.Items);
        Assert.Equal("Monitor Pro", product.Name);
        Assert.Equal(1899.00m, product.Price);
    }

    [Fact]
    public async Task UpdateAsync_WithUnknownId_PropagatesNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(99, Draft()));
    }

    [Fact]
    public async Task CreateAsync_AnnouncesTheChange()
    {
        await _service.CreateAsync(Draft("Monitor"));

        var notification = Assert.Single(_bus.Published);
        Assert.Equal(ProductChange.Created, notification.Change);
        Assert.Equal("Monitor", notification.ProductName);
    }

    [Fact]
    public async Task UpdateAsync_AnnouncesTheChange()
    {
        _repository.Items.Add(Product.Create("Antigo", null, 10m, ProductCategory.Toys, 1));

        await _service.UpdateAsync(0, Draft("Monitor Pro"));

        var notification = Assert.Single(_bus.Published);
        Assert.Equal(ProductChange.Updated, notification.Change);
    }

    [Fact]
    public async Task DeleteAsync_AnnouncesTheChange()
    {
        _repository.Items.Add(Product.Create("Monitor", null, 10m, ProductCategory.Toys, 1));

        await _service.DeleteAsync(0);

        var notification = Assert.Single(_bus.Published);
        Assert.Equal(ProductChange.Deleted, notification.Change);
        Assert.Equal("Monitor", notification.ProductName);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDraft_AnnouncesNothing()
    {
        var invalid = new ProductDraft("Monitor", null, -1m, ProductCategory.Electronics, 8);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.CreateAsync(invalid));

        Assert.Empty(_bus.Published);
    }

    [Fact]
    public async Task UpdateAsync_ReportsOnlyWhatActuallyChanged()
    {
        _repository.Items.Add(Product.Create("Monitor", "Painel IPS", 1899m, ProductCategory.Electronics, 8));

        await _service.UpdateAsync(0, new ProductDraft(
            "Monitor", "Painel IPS", 1499m, ProductCategory.Electronics, 8));

        var change = Assert.Single(Assert.Single(_bus.Published).Changes);
        Assert.Equal("Preço", change.Field);
        Assert.Equal("1.899,00", change.Before);
        Assert.Equal("1.499,00", change.After);
    }

    [Fact]
    public async Task UpdateAsync_ReportsEveryChangedField()
    {
        _repository.Items.Add(Product.Create("Antigo", null, 10m, ProductCategory.Toys, 1));

        await _service.UpdateAsync(0, new ProductDraft(
            "Novo", "Ficha nova", 25m, ProductCategory.Apparel, 9));

        var changes = Assert.Single(_bus.Published).Changes;

        Assert.Equal(
            ["Nome", "Descrição", "Preço", "Categoria", "Estoque"],
            changes.Select(change => change.Field));

        var category = changes.Single(change => change.Field == "Categoria");
        Assert.Equal("Brinquedos", category.Before);
        Assert.Equal("Vestuário", category.After);
    }

    [Fact]
    public async Task UpdateAsync_WithoutRealChanges_ReportsNothing()
    {
        _repository.Items.Add(Product.Create("Monitor", "Painel IPS", 1899m, ProductCategory.Electronics, 8));

        await _service.UpdateAsync(0, new ProductDraft(
            "Monitor", "Painel IPS", 1899m, ProductCategory.Electronics, 8));

        Assert.Empty(Assert.Single(_bus.Published).Changes);
    }

    [Fact]
    public async Task UpdateAsync_MarksAnAbsentPreviousValue()
    {
        _repository.Items.Add(Product.Create("Monitor", null, 1899m, ProductCategory.Electronics, 8));

        await _service.UpdateAsync(0, new ProductDraft(
            "Monitor", "Agora tem ficha", 1899m, ProductCategory.Electronics, 8));

        var change = Assert.Single(Assert.Single(_bus.Published).Changes);
        Assert.Equal("(vazio)", change.Before);
        Assert.Equal("Agora tem ficha", change.After);
    }

    [Fact]
    public async Task CreateAsync_ReportsTheInitialValuesWithoutAPreviousOne()
    {
        await _service.CreateAsync(Draft("Monitor", "Painel IPS"));

        var changes = Assert.Single(_bus.Published).Changes;

        Assert.All(changes, change => Assert.Null(change.Before));
        Assert.Equal(["Preço", "Categoria", "Estoque", "Descrição"], changes.Select(c => c.Field));
    }

    [Fact]
    public async Task CreateAsync_WithoutDescription_OmitsThatField()
    {
        await _service.CreateAsync(Draft("Monitor"));

        var fields = Assert.Single(_bus.Published).Changes.Select(change => change.Field);

        Assert.DoesNotContain("Descrição", fields);
    }

    [Fact]
    public async Task DeleteAsync_ReportsNoFieldDetail()
    {
        _repository.Items.Add(Product.Create("Monitor", null, 10m, ProductCategory.Toys, 1));

        await _service.DeleteAsync(0);

        Assert.Empty(Assert.Single(_bus.Published).Changes);
    }

    [Fact]
    public async Task DeleteAsync_DelegatesToTheRepository()
    {
        _repository.Items.Add(Product.Create("Monitor", null, 10m, ProductCategory.Toys, 1));

        await _service.DeleteAsync(0);

        Assert.Empty(_repository.Items);
    }

    private sealed class RecordingBus : IServiceBus<ProductChangedNotification>
    {
        public List<ProductChangedNotification> Published { get; } = [];

        public Task PublishAsync(ProductChangedNotification message)
        {
            Published.Add(message);
            return Task.CompletedTask;
        }

        public Task SubscribeAsync(Func<ProductChangedNotification, Task> handler) => Task.CompletedTask;
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Items { get; } = [];

        public List<string> Calls { get; } = [];

        public (string Term, int PageNumber, int PageSize) LastQuery { get; private set; }

        public Task<Product> GetByIdAsync(int id)
        {
            Calls.Add(nameof(GetByIdAsync));

            return id >= 0 && id < Items.Count
                ? Task.FromResult(Items[id])
                : throw new KeyNotFoundException($"Produto {id} não encontrado.");
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            Calls.Add(nameof(GetAllAsync));
            return Task.FromResult<IEnumerable<Product>>(Items);
        }

        public Task<IReadOnlyList<Product>> ListAsync(string term)
        {
            Calls.Add(nameof(ListAsync));
            return Task.FromResult<IReadOnlyList<Product>>(Items);
        }

        public Task<PagedResult<Product>> GetPageAsync(string term, int pageNumber, int pageSize)
        {
            Calls.Add(nameof(GetPageAsync));
            LastQuery = (term, pageNumber, pageSize);

            return Task.FromResult(new PagedResult<Product>(Items, Items.Count, pageNumber, pageSize));
        }

        public Task AddAsync(Product entity)
        {
            Calls.Add(nameof(AddAsync));
            Items.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Product entity)
        {
            Calls.Add(nameof(UpdateAsync));
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            Calls.Add(nameof(DeleteAsync));

            if (id < 0 || id >= Items.Count)
            {
                throw new KeyNotFoundException($"Produto {id} não encontrado.");
            }

            Items.RemoveAt(id);
            return Task.CompletedTask;
        }
    }
}
