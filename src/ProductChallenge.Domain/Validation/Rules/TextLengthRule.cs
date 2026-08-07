using ProductChallenge.Domain.Metadata;

namespace ProductChallenge.Domain.Validation.Rules;

public sealed class TextLengthRule : IValidationRule
{
    public Type AttributeType => typeof(TextLengthAttribute);

    public ValidationFailure? Check(Attribute attribute, ValidatedProperty property, object? value)
    {
        if (value is not string text || text.Length == 0)
        {
            return null;
        }

        var rule = (TextLengthAttribute)attribute;

        if (text.Length >= rule.Min && text.Length <= rule.Max)
        {
            return null;
        }

        var message = rule.ErrorMessage ?? (rule.Min > 0
            ? $"Informe entre {rule.Min} e {rule.Max} caracteres."
            : $"Informe no máximo {rule.Max} caracteres.");

        return new ValidationFailure(property.Name, message);
    }
}
