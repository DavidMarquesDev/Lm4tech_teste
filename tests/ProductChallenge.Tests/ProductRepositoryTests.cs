using ProductChallenge.Domain;
using ProductChallenge.Infrastructure.Repositories;

namespace ProductChallenge.Tests;

public class ProductRepositoryTests : IDisposable
{
    private readonly SqliteInMemoryDatabase _database = new();
    private readonly ProductRepository _repository;

    public ProductRepositoryTests() => _repository = new ProductRepository(_database);

    public void Dispose() => _database.Dispose();

    /// <summary>Nomes com prefixo numérico garantem ordenação previsível entre as páginas.</summary>
    private async Task SeedAsync(int quantity)
    {
        for (var index = 1; index <= quantity; index++)
        {
            var category = index % 2 == 0 ? ProductCategory.Electronics : ProductCategory.Toys;
            await _repository.AddAsync(
                Product.Create($"Produto {index:D3}", $"Lote {index}", 10m + index, category, index));
        }
    }

    [Fact]
    public async Task GetPageAsync_ReturnsOnlyTheRequestedSlice()
    {
        await SeedAsync(100);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber: 1, pageSize: 10);

        Assert.Equal(10, page.Items.Count);
        Assert.Equal("Produto 001", page.Items[0].Name);
        Assert.Equal("Produto 010", page.Items[9].Name);
    }

    [Fact]
    public async Task GetPageAsync_TotalCountCoversEverything_NotJustThePage()
    {
        await SeedAsync(100);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber: 1, pageSize: 10);

        Assert.Equal(100, page.TotalCount);
        Assert.Equal(10, page.PageCount);
    }

    [Fact]
    public async Task GetPageAsync_SecondPage_ContinuesWhereTheFirstStopped()
    {
        await SeedAsync(100);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber: 2, pageSize: 10);

        Assert.Equal("Produto 011", page.Items[0].Name);
        Assert.Equal("Produto 020", page.Items[9].Name);
    }

    [Fact]
    public async Task GetPageAsync_LastPage_MayComeIncomplete()
    {
        await SeedAsync(95);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber: 10, pageSize: 10);

        Assert.Equal(5, page.Items.Count);
        Assert.False(page.HasNextPage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task GetPageAsync_WithPageBelowOne_FallsBackToTheFirst(int pageNumber)
    {
        await SeedAsync(30);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber, pageSize: 10);

        Assert.Equal(1, page.PageNumber);
        Assert.Equal("Produto 001", page.Items[0].Name);
    }

    [Fact]
    public async Task GetPageAsync_BeyondTheLastPage_ClampsInsteadOfReturningNothing()
    {
        await SeedAsync(25);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber: 99, pageSize: 10);

        Assert.Equal(3, page.PageNumber);
        Assert.Equal(5, page.Items.Count);
    }

    [Fact]
    public async Task GetPageAsync_WithoutResults_StaysOnPageOne()
    {
        var page = await _repository.GetPageAsync("inexistente", pageNumber: 4, pageSize: 10);

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
        Assert.Equal(1, page.PageNumber);
        Assert.Equal(1, page.PageCount);
    }

    [Fact]
    public async Task GetPageAsync_PageSizeLargerThanTotal_ReturnsASinglePage()
    {
        await SeedAsync(7);

        var page = await _repository.GetPageAsync(string.Empty, pageNumber: 1, pageSize: 100);

        Assert.Equal(7, page.Items.Count);
        Assert.Equal(1, page.PageCount);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async Task GetPageAsync_FilterAppliesBeforeCounting()
    {
        await SeedAsync(100);

        var page = await _repository.GetPageAsync("Produto 01", pageNumber: 1, pageSize: 10);

        Assert.Equal(10, page.TotalCount);
        Assert.Equal(1, page.PageCount);
        Assert.All(page.Items, product => Assert.StartsWith("Produto 01", product.Name));
    }

    [Fact]
    public async Task GetPageAsync_FilterAndPagingWorkTogether()
    {
        await SeedAsync(100);

        var page = await _repository.GetPageAsync("Lote", pageNumber: 4, pageSize: 25);

        Assert.Equal(100, page.TotalCount);
        Assert.Equal(4, page.PageCount);
        Assert.Equal(25, page.Items.Count);
        Assert.Equal("Produto 076", page.Items[0].Name);
    }
}
