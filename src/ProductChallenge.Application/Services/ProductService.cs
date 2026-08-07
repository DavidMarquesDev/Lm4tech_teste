using ProductChallenge.Application.Abstractions;
using ProductChallenge.Domain;

namespace ProductChallenge.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IReadOnlyList<Product>> ListAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return (await _repository.GetAllAsync()).ToList();
        }

        return await _repository.SearchAsync(searchTerm);
    }

    public Task CreateAsync(ProductDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var product = Product.Create(
            draft.Name, draft.Description, draft.Price, draft.Category, draft.StockQuantity);

        return _repository.AddAsync(product);
    }

    public async Task UpdateAsync(int id, ProductDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        var product = await _repository.GetByIdAsync(id);
        product.SetDetails(
            draft.Name, draft.Description, draft.Price, draft.Category, draft.StockQuantity);

        await _repository.UpdateAsync(product);
    }

    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
}
