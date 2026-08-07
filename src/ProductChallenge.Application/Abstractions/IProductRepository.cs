using ProductChallenge.Domain;

namespace ProductChallenge.Application.Abstractions;

/// <summary>
/// O repositório genérico cobre o CRUD, mas não tem como expressar uma consulta de domínio.
/// A busca por texto entra aqui em vez de virar filtro em memória sobre
/// <see cref="IRepository{T}.GetAllAsync"/>.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> SearchAsync(string term);
}
