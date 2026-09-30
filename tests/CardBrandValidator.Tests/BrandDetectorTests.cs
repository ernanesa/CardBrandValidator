namespace CardBrandValidator.Tests;

using CardBrandValidator.Core.Domain;
using CardBrandValidator.Core.Services;
using FluentAssertions;

public class BrandDetectorTests
{
    [Theory]
    // Elo (Prefixos específicos testados antes de Visa e Mastercard)
    [InlineData("5067220000000009", CardBrand.Elo, "Elo")]
    [InlineData("4011780000000000", CardBrand.Elo, "Elo")]
    [InlineData("6504050000000000", CardBrand.Elo, "Elo")]

    // Hipercard
    [InlineData("6062822600000003", CardBrand.Hipercard, "HiperCard")]
    [InlineData("3841000000000000", CardBrand.Hipercard, "HiperCard")]

    // enRoute (15 dígitos)
    [InlineData("201400000000000", CardBrand.EnRoute, "enRoute")]
    [InlineData("214900000000000", CardBrand.EnRoute, "enRoute")]

    // Voyager (15 dígitos)
    [InlineData("869900000000000", CardBrand.Voyager, "Voyager")]

    // Amex (15 dígitos)
    [InlineData("340000000000009", CardBrand.Amex, "American Express")]
    [InlineData("378282246310005", CardBrand.Amex, "American Express")]

    // Diners (14 dígitos)
    [InlineData("30000000000004", CardBrand.Diners, "Diners Club")]
    [InlineData("36000000000014", CardBrand.Diners, "Diners Club")]
    [InlineData("30500000000000", CardBrand.Diners, "Diners Club")]

    // JCB (16 a 19 dígitos)
    [InlineData("3528000000000007", CardBrand.Jcb, "JCB")]
    [InlineData("3589000000000000", CardBrand.Jcb, "JCB")]

    // Discover (16 dígitos)
    [InlineData("6011000990139424", CardBrand.Discover, "Discover")]
    [InlineData("6440000000000000", CardBrand.Discover, "Discover")]

    // Aura (prefixo 50 não-Elo)
    [InlineData("5000000000000000", CardBrand.Aura, "Aura")]

    // MasterCard (16 dígitos)
    [InlineData("5100000000000008", CardBrand.MasterCard, "MasterCard")]
    [InlineData("5502098322341109", CardBrand.MasterCard, "MasterCard")]
    [InlineData("2221000000000000", CardBrand.MasterCard, "MasterCard")]

    // Visa (13 e 16 dígitos)
    [InlineData("4532015112830366", CardBrand.Visa, "Visa")]
    [InlineData("4123456789012", CardBrand.Visa, "Visa")] // 13 dígitos
    public void Detect_WithSupportedBrands_IdentifiesCorrectly(string cardNumber, CardBrand expectedBrand, string expectedName)
    {
        // Act
        BrandMetadata result = BrandDetector.Detect(cardNumber);

        // Assert
        result.Brand.Should().Be(expectedBrand);
        result.DisplayName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("9999000000000000")]
    [InlineData("1111222233334444")]
    [InlineData("")]
    [InlineData(null)]
    public void Detect_WithUnsupportedOrEmpty_ReturnsUnknown(string? cardNumber)
    {
        // Act
        BrandMetadata result = BrandDetector.Detect(cardNumber!);

        // Assert
        result.Brand.Should().Be(CardBrand.Unknown);
        result.DisplayName.Should().Be("Desconhecida");
    }

    [Fact]
    public void NonLuhnBrands_EnRouteAndVoyager_AreMarkedProperly()
    {
        // Act & Assert
        BrandDetector.GetMetadata(CardBrand.EnRoute).RequiresLuhn.Should().BeFalse();
        BrandDetector.GetMetadata(CardBrand.Voyager).RequiresLuhn.Should().BeFalse();
        BrandDetector.GetMetadata(CardBrand.Visa).RequiresLuhn.Should().BeTrue();
    }

    [Fact]
    public void GetAllSupportedBrands_ReturnsAll11Brands()
    {
        // Act
        var brands = BrandDetector.GetAllSupportedBrands();

        // Assert
        brands.Should().HaveCount(11);
        foreach (var brand in brands)
        {
            brand.ValidLengths.Should().NotBeNull().And.NotBeEmpty();
            brand.FormatPattern.Should().NotBeNull().And.NotBeEmpty();
        }
    }

    [Fact]
    public void GetMetadata_WithUnknownBrand_ReturnsUnknownMetadata()
    {
        // Act
        var meta = BrandDetector.GetMetadata(CardBrand.Unknown);

        // Assert
        meta.Brand.Should().Be(CardBrand.Unknown);
        meta.DisplayName.Should().Be("Desconhecida");
    }
}
