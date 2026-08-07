namespace ProductChallenge.Domain.Metadata;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NumberRangeAttribute : Attribute
{
    public double Min { get; init; } = double.MinValue;

    public double Max { get; init; } = double.MaxValue;

    public string? ErrorMessage { get; set; }
}
