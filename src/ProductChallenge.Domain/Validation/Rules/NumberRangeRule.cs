using System.Globalization;
using ProductChallenge.Domain.Metadata;

namespace ProductChallenge.Domain.Validation.Rules;

public sealed class NumberRangeRule : IValidationRule
{
    public Type AttributeType => typeof(NumberRangeAttribute);

    public ValidationFailure? Check(Attribute attribute, ValidatedProperty property, object? value)
    {
        if (value is not IConvertible convertible || value is string or bool)
        {
            return null;
        }

        var rule = (NumberRangeAttribute)attribute;
        var number = convertible.ToDouble(CultureInfo.InvariantCulture);

        if (number >= rule.Min && number <= rule.Max)
        {
            return null;
        }

        var message = rule.ErrorMessage
            ?? $"Informe um valor entre {rule.Min} e {rule.Max}.";

        return new ValidationFailure(property.Name, message);
    }
}
