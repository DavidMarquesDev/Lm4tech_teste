using ProductChallenge.Domain;

namespace ProductChallenge.Application.Abstractions;

/// <summary>
/// O repositório genérico cobre o CRUD, mas não tem como expressar uma consulta de domínio.
/// Filtro e paginação entram aqui para chegarem ao SQL, em vez de virarem trabalho em memória
/// sobre <see cref="IRepository{T}.GetAllAsync"/>.
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    /// <summary>
    /// Devolve a página efetivamente lida: um número fora da faixa é ajustado ao total
    /// disponível, de modo que a tela nunca fica numa página vazia depois de filtrar ou excluir.
    /// </summary>
    Task<PagedResult<Product>> GetPageAsync(string term, int pageNumber, int pageSize);
}
