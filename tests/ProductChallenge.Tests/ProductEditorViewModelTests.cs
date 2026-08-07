using System.Globalization;
using ProductChallenge.Models;
using ProductChallenge.ViewModels;

namespace ProductChallenge.Tests;

public class ProductEditorViewModelTests
{
    private static ProductEditorViewModel CreateEditor(
        string name = "Produto", string price = "10,00", string stock = "5",
        ProductCategory? category = ProductCategory.Electronics)
    {
        var editor = new ProductEditorViewModel
        {
            Name = name,
            Price = price,
            StockQuantity = stock,
            SelectedCategory = category is null ? null : ProductCategoryCatalog.Find(category.Value)
        };

        return editor;
    }

    public ProductEditorViewModelTests()
    {
        // As mensagens e a conversão numérica dependem da cultura corrente.
        CultureInfo.CurrentCulture = new CultureInfo("pt-BR");
    }

    [Fact]
    public void TryBuildDraft_WithEmptyForm_ReportsOneErrorPerField()
    {
        var editor = new ProductEditorViewModel();
        editor.StartNew();

        var draft = editor.TryBuildDraft();

        Assert.Null(draft);
        Assert.Equal(4, editor.Errors.Count);
        Assert.Contains(editor.Errors, e => e.FieldName == nameof(ProductEditorViewModel.Name));
        Assert.Contains(editor.Errors, e => e.FieldName == nameof(ProductEditorViewModel.Price));
        Assert.Contains(editor.Errors, e => e.FieldName == nameof(ProductEditorViewModel.StockQuantity));
        Assert.Contains(editor.Errors, e => e.FieldName == nameof(ProductEditorViewModel.SelectedCategory));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("10,00,00")]
    [InlineData("-3")]
    [InlineData("0")]
    public void TryBuildDraft_WithInvalidPrice_ReportsPriceFieldOnly(string price)
    {
        var editor = CreateEditor(price: price);

        var draft = editor.TryBuildDraft();

        Assert.Null(draft);
        var error = Assert.Single(editor.Errors);
        Assert.Equal(nameof(ProductEditorViewModel.Price), error.FieldName);
    }

    [Theory]
    [InlineData("1.234,56", 1234.56)]
    [InlineData("0,01", 0.01)]
    [InlineData("  99,90  ", 99.90)]
    public void TryBuildDraft_ParsesPriceUsingCurrentCulture(string input, double expected)
    {
        var editor = CreateEditor(price: input);

        var draft = editor.TryBuildDraft();

        Assert.NotNull(draft);
        Assert.Equal((decimal)expected, draft.Price);
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("2,5")]
    [InlineData("xyz")]
    public void TryBuildDraft_WithInvalidStock_ReportsStockFieldOnly(string stock)
    {
        var editor = CreateEditor(stock: stock);

        var draft = editor.TryBuildDraft();

        Assert.Null(draft);
        var error = Assert.Single(editor.Errors);
        Assert.Equal(nameof(ProductEditorViewModel.StockQuantity), error.FieldName);
    }

    [Fact]
    public void TryBuildDraft_WithValidData_ReturnsDraftWithoutErrors()
    {
        var editor = CreateEditor("  Monitor  ", "1.899,00", "8", ProductCategory.Electronics);

        var draft = editor.TryBuildDraft();

        Assert.NotNull(draft);
        Assert.Empty(editor.Errors);
        Assert.Equal("Monitor", draft.Name);
        Assert.Equal(1899.00m, draft.Price);
        Assert.Equal(8, draft.StockQuantity);
        Assert.Equal(ProductCategory.Electronics, draft.Category);
    }

    [Fact]
    public void StartEdit_LoadsFieldsAndEntersEditMode()
    {
        var product = Product.Create("Notebook", "Tela de 15 polegadas", 4599.90m, ProductCategory.Electronics, 12);
        var editor = new ProductEditorViewModel();

        editor.StartEdit(product);

        Assert.True(editor.IsEditing);
        Assert.Equal(product.Id, editor.EditingId);
        Assert.Equal("Notebook", editor.Name);
        Assert.Equal("4.599,90", editor.Price);
        Assert.Equal("12", editor.StockQuantity);
        Assert.Equal(ProductCategory.Electronics, editor.SelectedCategory?.Category);
    }

    [Fact]
    public void StartEdit_LoadsDescription()
    {
        var product = Product.Create("Notebook", "Tela de 15 polegadas", 4599.90m, ProductCategory.Electronics, 12);
        var editor = new ProductEditorViewModel();

        editor.StartEdit(product);

        Assert.Equal("Tela de 15 polegadas", editor.Description);
    }

    [Fact]
    public void StartEdit_WithoutDescription_LeavesFieldEmpty()
    {
        var product = Product.Create("Notebook", null, 4599.90m, ProductCategory.Electronics, 12);
        var editor = new ProductEditorViewModel();

        editor.StartEdit(product);

        Assert.Empty(editor.Description);
    }

    [Fact]
    public void TryBuildDraft_WithBlankDescription_ReturnsNullDescription()
    {
        var editor = CreateEditor();
        editor.Description = "   ";

        var draft = editor.TryBuildDraft();

        Assert.NotNull(draft);
        Assert.Null(draft.Description);
    }

    [Fact]
    public void TryBuildDraft_WithMultilineDescription_PreservesText()
    {
        var editor = CreateEditor();
        editor.Description = "Peso: 1,2 kg\r\nGarantia: 12 meses";

        var draft = editor.TryBuildDraft();

        Assert.NotNull(draft);
        Assert.Equal("Peso: 1,2 kg\r\nGarantia: 12 meses", draft.Description);
    }

    [Fact]
    public void TryBuildDraft_WithDescriptionAboveMaxLength_ReportsDescriptionFieldOnly()
    {
        var editor = CreateEditor();
        editor.Description = new string('a', Product.DescriptionMaxLength + 1);

        var draft = editor.TryBuildDraft();

        Assert.Null(draft);
        var error = Assert.Single(editor.Errors);
        Assert.Equal(nameof(ProductEditorViewModel.Description), error.FieldName);
    }

    [Fact]
    public void StartNew_ClearsFieldsAndErrors()
    {
        var editor = CreateEditor(name: string.Empty);
        editor.Description = "alguma coisa";
        editor.TryBuildDraft();

        editor.StartNew();

        Assert.False(editor.IsEditing);
        Assert.Null(editor.EditingId);
        Assert.Empty(editor.Name);
        Assert.Empty(editor.Description);
        Assert.Empty(editor.Price);
        Assert.Empty(editor.StockQuantity);
        Assert.Null(editor.SelectedCategory);
        Assert.Empty(editor.Errors);
    }

    [Fact]
    public void StartEdit_ThenTryBuildDraft_PreservesProductValues()
    {
        var product = Product.Create("Cafe", "Torrado e moido", 18.75m, ProductCategory.Groceries, 240);
        var editor = new ProductEditorViewModel();
        editor.StartEdit(product);

        var draft = editor.TryBuildDraft();

        Assert.NotNull(draft);
        Assert.Equal(product.Name, draft.Name);
        Assert.Equal(product.Price, draft.Price);
        Assert.Equal(product.StockQuantity, draft.StockQuantity);
        Assert.Equal(product.Category, draft.Category);
    }
}
