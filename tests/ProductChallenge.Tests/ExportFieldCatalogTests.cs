using ProductChallenge.Application.Reporting;
using ProductChallenge.Domain;

namespace ProductChallenge.Tests;

public class ExportFieldCatalogTests
{
    [Fact]
    public void For_OffersOnlyThePropertiesMarkedAsExportable()
    {
        var names = ExportFieldCatalog.For<Product>().Select(field => field.PropertyName).ToList();

        Assert.Equal(
            ["Id", "Name", "Category", "Price", "StockQuantity", "Description"],
            names);
    }

    [Fact]
    public void For_DoesNotOfferDerivedColumns()
    {
        var names = ExportFieldCatalog.For<Product>().Select(field => field.PropertyName);

        // SearchText existe para a busca funcionar sem acento; não é dado para o usuário exportar.
        Assert.DoesNotContain(nameof(Product.SearchText), names);
    }

    [Fact]
    public void For_UsesTheHeaderFromTheAttribute()
    {
        var field = ExportFieldCatalog.For<Product>().Single(f => f.PropertyName == nameof(Product.Name));

        Assert.Equal("Produto", field.Header);
    }

    [Fact]
    public void For_CarriesTheDeclaredFormat()
    {
        var price = ExportFieldCatalog.For<Product>().Single(f => f.PropertyName == nameof(Product.Price));
        var name = ExportFieldCatalog.For<Product>().Single(f => f.PropertyName == nameof(Product.Name));

        Assert.Equal("N2", price.Format);
        Assert.Null(name.Format);
    }

    [Fact]
    public void For_ReadsValuesFromTheInstance()
    {
        var product = Product.Create("Monitor", null, 1899m, ProductCategory.Electronics, 8);
        var field = ExportFieldCatalog.For<Product>().Single(f => f.PropertyName == nameof(Product.Name));

        Assert.Equal("Monitor", field.Read(product));
    }

    [Fact]
    public void For_ReturnsTheSameCachedDescriptionOnEveryCall()
    {
        Assert.Same(ExportFieldCatalog.For<Product>(), ExportFieldCatalog.For<Product>());
    }

    [Fact]
    public void For_TypeWithoutMarkedProperties_ReturnsEmpty()
    {
        Assert.Empty(ExportFieldCatalog.For<Untagged>());
    }

    private sealed class Untagged
    {
        public string Anything { get; set; } = string.Empty;
    }
}
