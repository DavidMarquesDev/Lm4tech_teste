using ProductChallenge.Application;
using ProductChallenge.Domain;
using ProductChallenge.Domain.Validation;

namespace ProductChallenge.Tests;

public class DynamicValidatorTests
{
    private static ProductInput ValidInput() => new()
    {
        Name = "Monitor",
        Price = 1899.00m,
        Category = ProductCategory.Electronics,
        StockQuantity = 8
    };

    [Fact]
    public void Validate_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => DynamicValidator.Validate(null!));
    }

    [Fact]
    public void Validate_WithValidInput_ReportsNothing()
    {
        var result = DynamicValidator.Validate(ValidInput());

        Assert.True(result.IsValid);
        Assert.Empty(result.Failures);
    }

    [Fact]
    public void Validate_WithEmptyInput_ReportsEveryRequiredField()
    {
        var result = DynamicValidator.Validate(new ProductInput());

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Failures.Count);
        Assert.Contains(result.Failures, failure => failure.PropertyName == nameof(ProductInput.Name));
        Assert.Contains(result.Failures, failure => failure.PropertyName == nameof(ProductInput.Price));
        Assert.Contains(result.Failures, failure => failure.PropertyName == nameof(ProductInput.Category));
        Assert.Contains(result.Failures, failure => failure.PropertyName == nameof(ProductInput.StockQuantity));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void RequiredRule_TreatsBlankTextAsMissing(string name)
    {
        var input = ValidInput();
        input.Name = name;

        var failure = Assert.Single(DynamicValidator.Validate(input).Failures);
        Assert.Equal(nameof(ProductInput.Name), failure.PropertyName);
    }

    [Fact]
    public void RequiredRule_AcceptsZeroBecauseTheTypeIsNullable()
    {
        var input = ValidInput();
        input.StockQuantity = 0;

        Assert.True(DynamicValidator.Validate(input).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1_000_001)]
    public void NumberRangeRule_RejectsValuesOutsideTheRange(double price)
    {
        var input = ValidInput();
        input.Price = (decimal)price;

        var failure = Assert.Single(DynamicValidator.Validate(input).Failures);
        Assert.Equal(nameof(ProductInput.Price), failure.PropertyName);
    }

    [Fact]
    public void NumberRangeRule_RejectsNegativeStock()
    {
        var input = ValidInput();
        input.StockQuantity = -1;

        var failure = Assert.Single(DynamicValidator.Validate(input).Failures);
        Assert.Equal(nameof(ProductInput.StockQuantity), failure.PropertyName);
    }

    [Fact]
    public void TextLengthRule_RejectsTextAboveTheMaximum()
    {
        var input = ValidInput();
        input.Description = new string('a', Product.DescriptionMaxLength + 1);

        var failure = Assert.Single(DynamicValidator.Validate(input).Failures);
        Assert.Equal(nameof(ProductInput.Description), failure.PropertyName);
    }

    [Fact]
    public void TextLengthRule_RejectsTextBelowTheMinimum()
    {
        var input = ValidInput();
        input.Name = "A";

        var failure = Assert.Single(DynamicValidator.Validate(input).Failures);
        Assert.Equal(nameof(ProductInput.Name), failure.PropertyName);
    }

    [Fact]
    public void TextLengthRule_IgnoresAbsentOptionalText()
    {
        var input = ValidInput();
        input.Description = null;

        Assert.True(DynamicValidator.Validate(input).IsValid);
    }

    [Fact]
    public void Validate_ReportsAtMostOneFailurePerProperty()
    {
        // Nome vazio dispara Required; o TextLength também poderia opinar sobre o tamanho.
        var input = ValidInput();
        input.Name = string.Empty;

        var failures = DynamicValidator.Validate(input).Failures;

        Assert.Single(failures);
    }

    [Fact]
    public void Validate_UsesTheMessageDeclaredInTheAttribute()
    {
        var input = ValidInput();
        input.Name = null;

        var failure = Assert.Single(DynamicValidator.Validate(input).Failures);
        Assert.Equal("Informe o nome do produto.", failure.Message);
    }

    [Fact]
    public void Validate_IgnoresTypesWithoutAttributes()
    {
        var result = DynamicValidator.Validate(new WithoutMetadata { Anything = null });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_IgnoresAttributesWithoutARule()
    {
        var result = DynamicValidator.Validate(new WithUnknownAttribute());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ReusesTheMetadataItAlreadyResolved()
    {
        var first = DynamicValidator.Validate(new ProductInput()).Failures.Count;
        var second = DynamicValidator.Validate(new ProductInput()).Failures.Count;

        Assert.Equal(first, second);
    }

    private sealed class WithoutMetadata
    {
        public string? Anything { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    private sealed class UnknownAttribute : Attribute;

    private sealed class WithUnknownAttribute
    {
        [Unknown]
        public string? Anything { get; set; }
    }
}
