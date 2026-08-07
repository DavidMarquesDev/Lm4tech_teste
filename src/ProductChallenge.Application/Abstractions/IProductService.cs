using ProductChallenge.Domain;

namespace ProductChallenge.Application.Abstractions;

public interface IProductService
{
    Task<IReadOnlyList<Product>> ListAsync(string searchTerm);

    Task CreateAsync(ProductDraft draft);

    Task UpdateAsync(int id, ProductDraft draft);

    Task DeleteAsync(int id);
}
