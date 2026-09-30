namespace CardBrandValidator.Tests;

using CardBrandValidator.Core.Algorithms;
using FluentAssertions;

public class LuhnAlgorithmTests
{
    [Theory]
    [InlineData("4532015112830366")] // Visa 16
    [InlineData("4123456789011")]    // Visa 13
    [InlineData("5502098322341104")] // MasterCard 16
    [InlineData("378282246310005")]  // Amex 15
    [InlineData("36000000000016")]   // Diners 14
    [InlineData("6011000990139424")] // Discover 16
    [InlineData("3528000000000007")] // JCB 16
    [InlineData("6062822600000003")] // Hipercard 16
    [InlineData("5067220000000003")] // Elo 16
    [InlineData("123456789015")]     // 12 digits valid Luhn
    public void IsValid_WithValidNumbers_ReturnsTrue(string validNumber)
    {
        // Act
        bool result = LuhnAlgorithm.IsValid(validNumber);
        bool spanResult = LuhnAlgorithm.IsValid(validNumber.AsSpan());

        // Assert
        result.Should().BeTrue();
        spanResult.Should().BeTrue();
    }

    [Theory]
    [InlineData("4532015112830367")] // Check digit +1
    [InlineData("5502098322341100")] // Check digit altered
    [InlineData("378282246310004")]  // Amex corrupted
    [InlineData("123456789014")]     // 12 digits invalid Luhn
    public void IsValid_WithInvalidCheckDigit_ReturnsFalse(string invalidNumber)
    {
        // Act
        bool result = LuhnAlgorithm.IsValid(invalidNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValid_WithNullOrEmpty_ReturnsFalse()
    {
        // Act & Assert
        LuhnAlgorithm.IsValid((string?)null).Should().BeFalse();
        LuhnAlgorithm.IsValid(string.Empty).Should().BeFalse();
        LuhnAlgorithm.IsValid(ReadOnlySpan<char>.Empty).Should().BeFalse();
    }

    [Theory]
    [InlineData("12345")] // Too short (< 12)
    [InlineData("12345678901")] // 11 digits (< 12)
    [InlineData("12345678901234567890")] // 20 digits (> 19)
    public void IsValid_WithInvalidLengths_ReturnsFalse(string outOfBoundsNumber)
    {
        // Act & Assert
        LuhnAlgorithm.IsValid(outOfBoundsNumber).Should().BeFalse();
    }

    [Theory]
    [InlineData("453201511283036A")]
    [InlineData("45320151-1283036")]
    [InlineData("45320151 1283036")]
    public void IsValid_WithNonDigitCharacters_ReturnsFalse(string nonDigitInput)
    {
        // Act & Assert
        LuhnAlgorithm.IsValid(nonDigitInput).Should().BeFalse();
    }
}
