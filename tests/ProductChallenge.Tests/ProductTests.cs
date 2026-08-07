using ProductChallenge.Domain;

namespace ProductChallenge.Tests;

public class ProductTests
{
    private static Product CreateValid(string? description = null) =>
        Product.Create("Produto", description, 10m, ProductCategory.Toys, 1);

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankName_Throws(string name)
    {
        Assert.Throws<ArgumentException>(
            () => Product.Create(name, null, 10m, ProductCategory.Toys, 1));
    }

    [Fact]
    public void Create_WithNameAboveMaxLength_Throws()
    {
        var name = new string('a', Product.NameMaxLength + 1);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Product.Create(name, null, 10m, ProductCategory.Toys, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1_000_001)]
    public void Create_WithPriceOutOfRange_Throws(double price)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Product.Create("Produto", null, (decimal)price, ProductCategory.Toys, 1));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(Product.StockMaxValue + 1)]
    public void Create_WithStockOutOfRange_Throws(int stock)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Product.Create("Produto", null, 10m, ProductCategory.Toys, stock));
    }

    [Fact]
    public void Create_WithUndefinedCategory_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Product.Create("Produto", null, 10m, (ProductCategory)99, 1));
    }

    [Fact]
    public void Create_WithDescriptionAboveMaxLength_Throws()
    {
        var description = new string('a', Product.DescriptionMaxLength + 1);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => Product.Create("Produto", description, 10m, ProductCategory.Toys, 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithoutDescription_StoresNull(string? description)
    {
        var product = CreateValid(description);

        Assert.Null(product.Description);
    }

    [Fact]
    public void Create_TrimsDescription()
    {
        var product = CreateValid("  Tela de 27 polegadas  ");

        Assert.Equal("Tela de 27 polegadas", product.Description);
    }

    [Fact]
    public void Create_PreservesLineBreaksInDescription()
    {
        var product = CreateValid("Linha 1\r\nLinha 2");

        Assert.Equal("Linha 1\r\nLinha 2", product.Description);
    }

    [Fact]
    public void Create_WithZeroStock_Succeeds()
    {
        var product = Product.Create("Esgotado", null, 10m, ProductCategory.Toys, 0);

        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void Create_TrimsName()
    {
        var product = Product.Create("  Mouse  ", null, 10m, ProductCategory.Electronics, 1);

        Assert.Equal("Mouse", product.Name);
    }

    [Theory]
    [InlineData(10.004, 10.00)]
    [InlineData(10.005, 10.01)]
    public void Create_RoundsPriceToTwoDecimals(double input, double expected)
    {
        var product = Product.Create("Teclado", null, (decimal)input, ProductCategory.Electronics, 1);

        Assert.Equal((decimal)expected, product.Price);
    }

    [Fact]
    public void SetDetails_UpdatesEveryField()
    {
        var product = Product.Create("Antigo", "Descrição antiga", 10m, ProductCategory.Toys, 1);

        product.SetDetails("Novo", "Descrição nova", 25.50m, ProductCategory.Apparel, 9);

        Assert.Equal("Novo", product.Name);
        Assert.Equal("Descrição nova", product.Description);
        Assert.Equal(25.50m, product.Price);
        Assert.Equal(ProductCategory.Apparel, product.Category);
        Assert.Equal(9, product.StockQuantity);
    }

    [Fact]
    public void SetDetails_WithInvalidValue_LeavesPreviousStateIntact()
    {
        var product = Product.Create("Original", "Original", 10m, ProductCategory.Toys, 5);

        Assert.Throws<ArgumentOutOfRangeException>(
            () => product.SetDetails("Alterado", "Alterado", -1m, ProductCategory.Apparel, 8));

        Assert.Equal("Original", product.Name);
        Assert.Equal("Original", product.Description);
        Assert.Equal(10m, product.Price);
        Assert.Equal(5, product.StockQuantity);
    }
}
