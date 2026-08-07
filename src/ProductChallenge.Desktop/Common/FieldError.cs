namespace ProductChallenge.Desktop.Common;

/// <summary>
/// <paramref name="FieldName"/> é o nome da propriedade do ViewModel, o que permite à View
/// localizar o controle correspondente sem conhecer as regras de validação.
/// </summary>
public sealed record FieldError(string FieldName, string Message);
