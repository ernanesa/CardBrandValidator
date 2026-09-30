namespace CardBrandValidator.Tests;

using CardBrandValidator.Core.Domain;
using CardBrandValidator.Core.Services;
using FluentAssertions;

public class CardValidatorTests
{
    [Theory]
    // Visa
    [InlineData("4532 0151 1283 0366", CardBrand.Visa, "Visa")]
    // MasterCard
    [InlineData("5502-0983-2234-1104", CardBrand.MasterCard, "MasterCard")]
    // Amex
    [InlineData("378282246310005", CardBrand.Amex, "American Express")]
    // Diners
    [InlineData("36000000000016", CardBrand.Diners, "Diners Club")]
    // Discover
    [InlineData("6011000990139424", CardBrand.Discover, "Discover")]
    // JCB
    [InlineData("3528000000000007", CardBrand.Jcb, "JCB")]
    // Hipercard
    [InlineData("6062822600000003", CardBrand.Hipercard, "HiperCard")]
    // Elo
    [InlineData("5067220000000003", CardBrand.Elo, "Elo")]
    // Aura
    [InlineData("5000000000000009", CardBrand.Aura, "Aura")]
    public void Validate_WithValidCards_ReturnsValidResult(string input, CardBrand expectedBrand, string expectedName)
    {
        // Act
        CardValidationResult result = CardValidator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be(expectedBrand);
        result.BrandName.Should().Be(expectedName);
        result.IsLuhnValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.CleanedNumber.Should().NotContain(" ").And.NotContain("-");
        result.MaskedNumber.Should().Contain("•");
        result.FormattedNumber.Should().Contain(" ");
    }

    [Theory]
    // enRoute (15 dígitos, prefixo 2014, dispensa Luhn)
    [InlineData("201400000000000", CardBrand.EnRoute, "enRoute")]
    // Voyager (15 dígitos, prefixo 8699, dispensa Luhn)
    [InlineData("869900000000000", CardBrand.Voyager, "Voyager")]
    public void Validate_WithNonLuhnHistoricalBrands_ReturnsValid(string input, CardBrand expectedBrand, string expectedName)
    {
        // Act
        CardValidationResult result = CardValidator.Validate(input);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Brand.Should().Be(expectedBrand);
        result.BrandName.Should().Be(expectedName);
        result.IsLuhnValid.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Theory]
    [InlineData("4532015112830367", CardBrand.Visa)] // Último dígito alterado (Luhn falha)
    [InlineData("5502098322341100", CardBrand.MasterCard)]
    [InlineData("378282246310004", CardBrand.Amex)]
    public void Validate_WithInvalidLuhn_ReturnsInvalidWithLuhnErrorMessage(string input, CardBrand expectedBrand)
    {
        // Act
        CardValidationResult result = CardValidator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be(expectedBrand);
        result.IsLuhnValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Algoritmo de Luhn");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithNullOrWhitespace_ReturnsInvalidWithEmptyMessage(string? input)
    {
        // Act
        CardValidationResult result = CardValidator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be(CardBrand.Unknown);
        result.ErrorMessage.Should().Contain("não pode ser vazio ou nulo");
    }

    [Theory]
    [InlineData("4532-ABCD-1234-5678")]
    [InlineData("4532@0151!1283#0366")]
    public void Validate_WithInvalidCharacters_ReturnsInvalidWithCharactersMessage(string input)
    {
        // Act
        CardValidationResult result = CardValidator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("caracteres inválidos");
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("12345678901")] // 11 dígitos
    [InlineData("12345678901234567890")] // 20 dígitos
    public void Validate_WithInvalidLength_ReturnsInvalidWithLengthMessage(string input)
    {
        // Act
        CardValidationResult result = CardValidator.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Comprimento inválido");
    }

    [Fact]
    public void Validate_WithUnknownBrandValidLuhn_ReturnsInvalidWithUnrecognizedBrandMessage()
    {
        // Arrange (9999000000000004 possui Luhn válido, mas prefixo 9999 não é mapeado)
        string unknownCardWithValidLuhn = "9999000000000004";

        // Act
        CardValidationResult result = CardValidator.Validate(unknownCardWithValidLuhn);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Brand.Should().Be(CardBrand.Unknown);
        result.IsLuhnValid.Should().BeTrue();
        result.ErrorMessage.Should().Contain("Bandeira não identificada");
    }
}
