namespace ProductChallenge.Domain.Metadata;

[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
    public RequiredAttribute()
    {
    }

    public RequiredAttribute(string errorMessage) => ErrorMessage = errorMessage;

    public string ErrorMessage { get; set; } = "Campo obrigatório.";
}
