using ProductChallenge.Domain.Metadata;

namespace ProductChallenge.Domain.Validation.Rules;

public sealed class RequiredRule : IValidationRule
{
    public Type AttributeType => typeof(RequiredAttribute);

    public ValidationFailure? Check(Attribute attribute, ValidatedProperty property, object? value)
    {
        // Os campos de entrada usam tipos anuláveis justamente para que "não preenchido" seja
        // null, e não se confunda com o zero de um decimal ou int.
        var missing = value switch
        {
            null => true,
            string text => string.IsNullOrWhiteSpace(text),
            System.Collections.ICollection items => items.Count == 0,
            _ => false
        };

        return missing
            ? new ValidationFailure(property.Name, ((RequiredAttribute)attribute).ErrorMessage)
            : null;
    }
}
