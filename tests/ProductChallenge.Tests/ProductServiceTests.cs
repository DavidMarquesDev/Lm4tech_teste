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
