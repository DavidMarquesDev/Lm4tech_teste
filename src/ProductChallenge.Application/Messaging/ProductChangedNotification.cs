namespace ProductChallenge.Application.Messaging;

public enum ProductChange
{
    Created = 1,
    Updated = 2,
    Deleted = 3
}

/// <summary>
/// <paramref name="Before"/> é nulo numa criação, onde só existe o valor novo.
/// </summary>
public sealed record FieldChange(string Field, string? Before, string? After);

public sealed record ProductChangedNotification(
    int ProductId,
    string ProductName,
    ProductChange Change,
    IReadOnlyList<FieldChange> Changes);
