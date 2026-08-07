using ProductChallenge.Domain;

namespace ProductChallenge.Application.Abstractions;

public interface IProductService
{
    Task<PagedResult<Product>> ListAsync(string searchTerm, int pageNumber, int pageSize);

    Task CreateAsync(ProductDraft draft);

    Task UpdateAsync(int id, ProductDraft draft);

    Task DeleteAsync(int id);
}
