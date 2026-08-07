using System.Globalization;
using Microsoft.EntityFrameworkCore;
using ProductChallenge.Application.Services;
using ProductChallenge.Desktop.ViewModels;
using ProductChallenge.Domain;
using ProductChallenge.Infrastructure.Repositories;

namespace ProductChallenge.Tests;

public class ProductListViewModelTests : IDisposable
{
    private readonly SqliteInMemoryDatabase _database = new();

    public ProductListViewModelTests() => CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

    public void Dispose() => _database.Dispose();

    private ProductListViewModel CreateViewModel() =>
        new(new ProductService(new ProductRepository(_database)));

    private static void FillEditor(
        ProductListViewModel viewModel, string name, string price, string stock, ProductCategory category,
        string description = "")
    {
        viewModel.Editor.Name = name;
        viewModel.Editor.Description = description;
        viewModel.Editor.Price = price;
        viewModel.Editor.StockQuantity = stock;
        viewModel.Editor.SelectedCategory = ProductCategoryCatalog.Find(category);
    }

    [Fact]
    public async Task Load_WithoutProducts_LeavesListEmpty()
    {
        var viewModel = CreateViewModel();

        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Empty(viewModel.Products);
    }

    [Fact]
    public async Task Save_WithValidData_PersistsAndRefreshesList()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Notebook", "4.599,90", "12", ProductCategory.Electronics);

        await viewModel.SaveCommand.ExecuteAsync(null);

        var product = Assert.Single(viewModel.Products);
        Assert.True(product.Id > 0);
        Assert.Equal("Notebook", product.Name);
        Assert.Equal(4599.90m, product.Price);
        Assert.Equal(ProductCategory.Electronics, product.Category);
        Assert.Equal(12, product.StockQuantity);
    }

    [Fact]
    public async Task Save_WithValidData_ClearsEditor()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Notebook", "4.599,90", "12", ProductCategory.Electronics);

        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.False(viewModel.Editor.IsEditing);
        Assert.Empty(viewModel.Editor.Name);
    }

    [Fact]
    public async Task Save_WithInvalidData_KeepsDataAndExposesErrors()
    {
        var viewModel = CreateViewModel();
        await viewModel.LoadCommand.ExecuteAsync(null);

        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.Empty(viewModel.Products);
        Assert.Equal(4, viewModel.Editor.Errors.Count);
    }

    [Fact]
    public async Task Save_InEditMode_UpdatesProductWithoutCreatingAnother()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Notebook", "4.599,90", "12", ProductCategory.Electronics);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SelectedProduct = viewModel.Products.Single();
        viewModel.StartEditCommand.Execute(null);
        FillEditor(viewModel, "Notebook Pro", "5.999,00", "3", ProductCategory.Electronics);
        await viewModel.SaveCommand.ExecuteAsync(null);

        var product = Assert.Single(viewModel.Products);
        Assert.Equal("Notebook Pro", product.Name);
        Assert.Equal(5999.00m, product.Price);
        Assert.Equal(3, product.StockQuantity);
    }

    [Fact]
    public async Task Delete_RemovesOnlySelectedProduct()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Arroz", "12,49", "300", ProductCategory.Groceries);
        await viewModel.SaveCommand.ExecuteAsync(null);
        FillEditor(viewModel, "Boneca", "89,90", "40", ProductCategory.Toys);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SelectedProduct = viewModel.Products.Single(p => p.Name == "Arroz");
        await viewModel.DeleteCommand.ExecuteAsync(null);

        var remaining = Assert.Single(viewModel.Products);
        Assert.Equal("Boneca", remaining.Name);
    }

    [Fact]
    public async Task Load_OrdersProductsByName()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Zebra de pelucia", "59,90", "5", ProductCategory.Toys);
        await viewModel.SaveCommand.ExecuteAsync(null);
        FillEditor(viewModel, "Arroz integral", "12,49", "300", ProductCategory.Groceries);
        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.Equal(
            ["Arroz integral", "Zebra de pelucia"],
            viewModel.Products.Select(p => p.Name));
    }

    [Fact]
    public void EditAndDeleteCommands_RequireSelectedProduct()
    {
        var viewModel = CreateViewModel();

        Assert.False(viewModel.StartEditCommand.CanExecute(null));
        Assert.False(viewModel.DeleteCommand.CanExecute(null));

        viewModel.SelectedProduct = Product.Create("Produto", null, 10m, ProductCategory.Toys, 1);

        Assert.True(viewModel.StartEditCommand.CanExecute(null));
        Assert.True(viewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public async Task Save_WhenEditedProductWasRemovedElsewhere_NotifiesWithoutThrowing()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Fantasma", "10,00", "1", ProductCategory.Toys);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SelectedProduct = viewModel.Products.Single();
        viewModel.StartEditCommand.Execute(null);

        await using (var context = _database.CreateDbContext())
        {
            await context.Products.ExecuteDeleteAsync();
        }

        var failures = new List<string>();
        viewModel.OperationFailed += (_, message) => failures.Add(message);
        FillEditor(viewModel, "Fantasma editado", "20,00", "2", ProductCategory.Toys);
        await viewModel.SaveCommand.ExecuteAsync(null);

        Assert.Single(failures);
        Assert.Empty(viewModel.Products);
    }

    [Fact]
    public async Task Save_PersistedData_SurvivesNewContext()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Cabo \"USB-C\", 2m", "39,90", "150", ProductCategory.HomeAndGarden);
        await viewModel.SaveCommand.ExecuteAsync(null);

        await using var context = _database.CreateDbContext();
        var stored = await context.Products.AsNoTracking().SingleAsync();

        Assert.Equal("Cabo \"USB-C\", 2m", stored.Name);
        Assert.Equal(39.90m, stored.Price);
    }

    [Fact]
    public async Task Save_PersistsDescription()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Monitor", "1.899,00", "8", ProductCategory.Electronics,
            "27 polegadas, 144 Hz\r\nEntradas: HDMI 2.1, DisplayPort");

        await viewModel.SaveCommand.ExecuteAsync(null);

        var product = Assert.Single(viewModel.Products);
        Assert.Equal("27 polegadas, 144 Hz\r\nEntradas: HDMI 2.1, DisplayPort", product.Description);
    }

    [Fact]
    public async Task Search_FiltersByName()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Notebook Dell", "4.599,90", "12", ProductCategory.Electronics);
        await viewModel.SaveCommand.ExecuteAsync(null);
        FillEditor(viewModel, "Arroz integral", "12,49", "300", ProductCategory.Groceries);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = "notebook";
        await viewModel.LoadCommand.ExecuteAsync(null);

        var product = Assert.Single(viewModel.Products);
        Assert.Equal("Notebook Dell", product.Name);
    }

    [Fact]
    public async Task Search_AlsoMatchesDescription()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Monitor", "1.899,00", "8", ProductCategory.Electronics, "Painel IPS de 27 polegadas");
        await viewModel.SaveCommand.ExecuteAsync(null);
        FillEditor(viewModel, "Arroz integral", "12,49", "300", ProductCategory.Groceries);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = "IPS";
        await viewModel.LoadCommand.ExecuteAsync(null);

        var product = Assert.Single(viewModel.Products);
        Assert.Equal("Monitor", product.Name);
    }

    [Theory]
    [InlineData("eletronico")]
    [InlineData("Eletrônico")]
    [InlineData("ELETRONICO")]
    [InlineData("eletrônico")]
    public async Task Search_IgnoresAccentsAndCase(string term)
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Componente eletrônico", "99,90", "5", ProductCategory.Electronics);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = term;
        await viewModel.LoadCommand.ExecuteAsync(null);

        var product = Assert.Single(viewModel.Products);
        Assert.Equal("Componente eletrônico", product.Name);
    }

    [Theory]
    [InlineData("acucar")]
    [InlineData("AÇÚCAR")]
    [InlineData("Açúcar")]
    public async Task Search_IgnoresCedillaAndTilde(string term)
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Açúcar refinado", "5,49", "80", ProductCategory.Groceries);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = term;
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Single(viewModel.Products);
    }

    [Fact]
    public async Task Search_WithoutAccents_AlsoMatchesDescription()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Monitor", "1.899,00", "8", ProductCategory.Electronics,
            "Resolução 4K com atualização de 144 Hz");
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = "resolucao";
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Single(viewModel.Products);
    }

    [Fact]
    public async Task Search_AfterEdit_UsesUpdatedText()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Cadeira", "450,00", "10", ProductCategory.HomeAndGarden);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SelectedProduct = viewModel.Products.Single();
        viewModel.StartEditCommand.Execute(null);
        FillEditor(viewModel, "Poltrona reclinável", "450,00", "10", ProductCategory.HomeAndGarden);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = "reclinavel";
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.Single(viewModel.Products);

        viewModel.SearchTerm = "cadeira";
        await viewModel.LoadCommand.ExecuteAsync(null);
        Assert.Empty(viewModel.Products);
    }

    [Fact]
    public async Task Search_WithoutMatches_ReportsTermInStatus()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Notebook Dell", "4.599,90", "12", ProductCategory.Electronics);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = "inexistente";
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Empty(viewModel.Products);
        Assert.Contains("inexistente", viewModel.StatusMessage);
    }

    [Fact]
    public async Task Search_WithBlankTerm_ReturnsEverything()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Notebook Dell", "4.599,90", "12", ProductCategory.Electronics);
        await viewModel.SaveCommand.ExecuteAsync(null);
        FillEditor(viewModel, "Arroz integral", "12,49", "300", ProductCategory.Groceries);
        await viewModel.SaveCommand.ExecuteAsync(null);

        viewModel.SearchTerm = "   ";
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Equal(2, viewModel.Products.Count);
    }


    private async Task SeedAsync(ProductListViewModel viewModel, int quantity)
    {
        for (var index = 1; index <= quantity; index++)
        {
            FillEditor(viewModel, $"Produto {index:D3}", "10,00", "5", ProductCategory.Toys);
            await viewModel.SaveCommand.ExecuteAsync(null);
        }
    }

    [Fact]
    public async Task Load_ReturnsOnlyOnePageAtATime()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 40);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Equal(10, viewModel.Products.Count);
        Assert.Equal(4, viewModel.PageCount);
        Assert.Contains("40 produtos", viewModel.StatusMessage);
    }

    [Fact]
    public async Task GoToNextPage_MovesForwardAndKeepsThePageSize()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 25);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);
        await viewModel.GoToNextPageCommand.ExecuteAsync(null);

        Assert.Equal(2, viewModel.PageNumber);
        Assert.Equal(10, viewModel.Products.Count);
        Assert.Equal("Produto 011", viewModel.Products[0].Name);
    }

    [Fact]
    public async Task Navigation_IsDisabledAtTheEdges()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 15);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.False(viewModel.GoToPreviousPageCommand.CanExecute(null));
        Assert.True(viewModel.GoToNextPageCommand.CanExecute(null));

        await viewModel.GoToNextPageCommand.ExecuteAsync(null);

        Assert.True(viewModel.GoToPreviousPageCommand.CanExecute(null));
        Assert.False(viewModel.GoToNextPageCommand.CanExecute(null));
    }

    [Fact]
    public async Task ChangingTheSearchTerm_ReturnsToTheFirstPage()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 30);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);
        await viewModel.GoToNextPageCommand.ExecuteAsync(null);
        Assert.Equal(2, viewModel.PageNumber);

        viewModel.SearchTerm = "Produto 02";

        Assert.Equal(1, viewModel.PageNumber);
    }

    [Fact]
    public async Task ChangingThePageSize_ReturnsToTheFirstPage()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 30);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);
        await viewModel.GoToNextPageCommand.ExecuteAsync(null);

        viewModel.PageSize = 30;

        Assert.Equal(1, viewModel.PageNumber);
    }

    [Fact]
    public async Task Delete_OnTheLastRemainingPage_LandsOnAValidPage()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 11);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);
        await viewModel.GoToNextPageCommand.ExecuteAsync(null);
        Assert.Equal(2, viewModel.PageNumber);

        viewModel.SelectedProduct = viewModel.Products.Single();
        await viewModel.DeleteCommand.ExecuteAsync(null);

        Assert.Equal(1, viewModel.PageNumber);
        Assert.Equal(10, viewModel.Products.Count);
    }

    [Fact]
    public async Task Search_CombinedWithPaging_CountsOnlyMatches()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 30);

        viewModel.PageSize = 10;
        viewModel.SearchTerm = "Produto 01";
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Equal(10, viewModel.Products.Count);
        Assert.Equal(1, viewModel.PageCount);
        Assert.Contains("10 produtos encontrados", viewModel.StatusMessage);
    }

    [Fact]
    public async Task PageSummary_ReportsTheCurrentPosition()
    {
        var viewModel = CreateViewModel();
        await SeedAsync(viewModel, 25);

        viewModel.PageSize = 10;
        await viewModel.LoadCommand.ExecuteAsync(null);

        Assert.Equal("Página 1 de 3", viewModel.PageSummary);

        await viewModel.GoToNextPageCommand.ExecuteAsync(null);

        Assert.Equal("Página 2 de 3", viewModel.PageSummary);
    }

    [Fact]
    public async Task Mapping_StoresCategoryAsText()
    {
        var viewModel = CreateViewModel();
        FillEditor(viewModel, "Camiseta", "49,90", "20", ProductCategory.Apparel);
        await viewModel.SaveCommand.ExecuteAsync(null);

        await using var context = _database.CreateDbContext();
        var category = await context.Database
            .SqlQuery<string>($"SELECT Category AS Value FROM Products LIMIT 1")
            .SingleAsync();

        Assert.Equal(nameof(ProductCategory.Apparel), category);
    }
}
