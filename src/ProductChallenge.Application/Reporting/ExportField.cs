namespace ProductChallenge.Application.Reporting;

public sealed record ExportField(
    string PropertyName,
    string Header,
    string? Format,
    Func<object, object?> Read);
