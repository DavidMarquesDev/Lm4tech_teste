namespace ProductChallenge.Domain.Metadata;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TextLengthAttribute : Attribute
{
    public int Min { get; init; }

    public int Max { get; init; } = int.MaxValue;

    public string? ErrorMessage { get; set; }
}
