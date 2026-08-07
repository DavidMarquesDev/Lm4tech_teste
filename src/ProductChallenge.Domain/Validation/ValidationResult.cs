namespace ProductChallenge.Domain.Validation;

public sealed record ValidationFailure(string PropertyName, string Message);

public sealed class ValidationResult
{
    private static readonly ValidationResult Valid = new([]);

    private ValidationResult(IReadOnlyList<ValidationFailure> failures) => Failures = failures;

    public IReadOnlyList<ValidationFailure> Failures { get; }

    public bool IsValid => Failures.Count == 0;

    public static ValidationResult Failed(IReadOnlyList<ValidationFailure> failures) =>
        failures.Count == 0 ? Valid : new ValidationResult(failures);
}
