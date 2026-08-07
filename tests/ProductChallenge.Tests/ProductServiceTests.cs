using ProductChallenge.Application;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Application.Services;
using ProductChallenge.Domain;

namespace ProductChallenge.Tests;

/// <summary>
/// Sem banco: o serviço só depende de <see cref="IProductRepository"/>, então um dublê basta.
/// </summary>
public class ProductServiceTests
{
    private readonly FakeProductRepository _repository = new();
    private readonly ProductService _service;

    public ProductServiceTests() => _service = new ProductService(_repository);

    private static ProductDraft Draft(string name = "Monitor", string? description = null) =>
        new(name, description, 1899.00m, ProductCategory.Electronics, 8);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ListAsync_WithBlankTerm_AsksForEverything(string term)
    {
        await _service.ListAsync(term);

        Assert.Equal([nameof(IProductRepository.GetAllAsync)], _repository.Calls);
    }

    [Fact]
    public async Task ListAsync_WithTerm_DelegatesToSearch()
    {
        await _service.ListAsync("monitor");

        Assert.Equal([nameof(IProductRepository.SearchAsync)], _repository.Calls);
        Assert.Equal("monitor", _repository.LastSearchTerm);
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
    public async Task DeleteAsync_DelegatesToTheRepository()
    {
        _repository.Items.Add(Product.Create("Monitor", null, 10m, ProductCategory.Toys, 1));

        await _service.DeleteAsync(0);

        Assert.Empty(_repository.Items);
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Items { get; } = [];

        public List<string> Calls { get; } = [];

        public string? LastSearchTerm { get; private set; }

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

        public Task<IReadOnlyList<Product>> SearchAsync(string term)
        {
            Calls.Add(nameof(SearchAsync));
            LastSearchTerm = term;
            return Task.FromResult<IReadOnlyList<Product>>(Items);
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
