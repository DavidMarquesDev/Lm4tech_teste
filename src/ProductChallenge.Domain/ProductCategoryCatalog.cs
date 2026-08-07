namespace ProductChallenge.Domain;

public static class ProductCategoryCatalog
{
    private static readonly CategoryOption[] AllOptions =
    [
        new CategoryOption(ProductCategory.Electronics, "Eletrônicos"),
        new CategoryOption(ProductCategory.Groceries, "Alimentos"),
        new CategoryOption(ProductCategory.Apparel, "Vestuário"),
        new CategoryOption(ProductCategory.HomeAndGarden, "Casa e Jardim"),
        new CategoryOption(ProductCategory.Toys, "Brinquedos")
    ];

    public static IReadOnlyList<CategoryOption> Options => AllOptions;

    public static CategoryOption? Find(ProductCategory category) =>
        Array.Find(AllOptions, option => option.Category == category);

    public static string LabelFor(ProductCategory category) =>
        Find(category)?.Label ?? category.ToString();
}
