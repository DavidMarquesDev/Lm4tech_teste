
namespace ProductChallenge.Domain.Validation.Rules;

/// <summary>
/// Uma regra por atributo. Suportar um novo atributo passa a ser acrescentar uma implementação,
/// e não editar o validador.
/// </summary>
public interface IValidationRule
{
    Type AttributeType { get; }

    ValidationFailure? Check(Attribute attribute, ValidatedProperty property, object? value);
}
