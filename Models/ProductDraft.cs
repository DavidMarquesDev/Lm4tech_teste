namespace ProductChallenge.Models;

public sealed record ProductDraft(
    string Name,
    string? Description,
    decimal Price,
    ProductCategory Category,
    int StockQuantity);
