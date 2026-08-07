using Microsoft.EntityFrameworkCore;
using ProductChallenge.Application;
using ProductChallenge.Application.Abstractions;
using ProductChallenge.Domain;
using ProductChallenge.Infrastructure.Persistence;

namespace ProductChallenge.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ProductRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
    }

    /// <summary>
    /// O contrato do desafio devolve <c>Task&lt;T&gt;</c> não anulável, então a ausência é
    /// sinalizada por exceção em vez de <c>null</c>.
    /// </summary>
    public async Task<Product> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == id)
               ?? throw new KeyNotFoundException($"Produto {id} não encontrado.");
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        return await context.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync();
    }

    public async Task<PagedResult<Product>> GetPageAsync(string term, int pageNumber, int pageSize)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var query = context.Products.AsNoTracking();
        var normalizedTerm = SearchNormalizer.Normalize(term);

        if (normalizedTerm.Length > 0)
        {
            var pattern = $"%{normalizedTerm}%";
            query = query.Where(product => EF.Functions.Like(product.SearchText, pattern));
        }

        var totalCount = await query.CountAsync();

        // Ajusta a página ao total antes de paginar: pedir a página 5 de um resultado com 2
        // devolveria uma lista vazia sem explicação.
        var pageCount = totalCount == 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var page = Math.Clamp(pageNumber, 1, pageCount);

        var items = await query
            .OrderBy(product => product.Name)
            .ThenBy(product => product.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Add(entity);

        await SaveAsync(context, "Não foi possível gravar o produto.");
    }

    public async Task UpdateAsync(Product entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Products.Update(entity);

        await SaveAsync(context, "Não foi possível atualizar o produto.");
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        int affectedRows;

        try
        {
            affectedRows = await context.Products
                .Where(product => product.Id == id)
                .ExecuteDeleteAsync();
        }
        catch (DbUpdateException exception)
        {
            throw new DataAccessException("Não foi possível excluir o produto.", exception);
        }

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"Produto {id} não encontrado.");
        }
    }

    private static async Task SaveAsync(AppDbContext context, string message)
    {
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException exception)
        {
            throw new DataAccessException(message, exception);
        }
    }
}
