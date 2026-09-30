namespace CardBrandValidator.Tests;

using CardBrandValidator.Core.Services;
using FluentAssertions;

public class CardMaskerTests
{
    [Fact]
    public void Mask_WithStandard16Digits_PreservesFirstAndLastFour()
    {
        // Arrange
        string clean = "4532015002734518";

        // Act
        string masked = CardMasker.Mask(clean, [4, 4, 4, 4]);

        // Assert
        masked.Should().Be("4532 •••• •••• 4518");
    }

    [Fact]
    public void Mask_WithDefaultPatternNull_AppliesStandardPattern()
    {
        // Arrange
        string clean = "4532015002734518";

        // Act
        string masked = CardMasker.Mask(clean, null);

        // Assert
        masked.Should().Be("4532 •••• •••• 4518");
    }

    [Fact]
    public void Mask_WithAmex15Digits_FormatsWithAmexPattern()
    {
        // Arrange
        string clean = "378282246310005";

        // Act
        string masked = CardMasker.Mask(clean, [4, 6, 5]);

        // Assert
        masked.Should().Be("3782 •••••• •0005");
    }

    [Fact]
    public void Mask_WithDiners14Digits_FormatsWithDinersPattern()
    {
        // Arrange
        string clean = "36000000000014";

        // Act
        string masked = CardMasker.Mask(clean, [4, 6, 4]);

        // Assert
        masked.Should().Be("3600 •••••• 0014");
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("1234", "1234")]
    [InlineData("1234567", "1234567")]
    public void Mask_WithShortOrEmptyInput_ReturnsSafeFallback(string? input, string expected)
    {
        // Act
        string result = CardMasker.Mask(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Format_With16Digits_AppliesBlockSpacing()
    {
        // Arrange
        string clean = "5502098322341109";

        // Act
        string formatted = CardMasker.Format(clean, [4, 4, 4, 4]);

        // Assert
        formatted.Should().Be("5502 0983 2234 1109");
    }

    [Fact]
    public void Format_WithDefaultPatternNull_AppliesStandardPattern()
    {
        // Arrange
        string clean = "5502098322341109";

        // Act
        string formatted = CardMasker.Format(clean, null);

        // Assert
        formatted.Should().Be("5502 0983 2234 1109");
    }

    [Fact]
    public void Format_WithNullOrEmpty_ReturnsEmpty()
    {
        // Act & Assert
        CardMasker.Format(null).Should().BeEmpty();
        CardMasker.Format(string.Empty).Should().BeEmpty();
    }

    [Fact]
    public void Format_WithRemainingDigitsBeyondPattern_AppendsCorrectly()
    {
        // Arrange (19 digits with 4-4-4-4 pattern has 3 remaining digits)
        string clean = "1234567812345678999";

        // Act
        string formatted = CardMasker.Format(clean, [4, 4, 4, 4]);

        // Assert
        formatted.Should().Be("1234 5678 1234 5678 999");
    }

    [Fact]
    public void Format_WithInputShorterThanPattern_BreaksEarly()
    {
        // Arrange (8 digits with 4-4-4-4 pattern stops at second block)
        string clean = "12345678";

        // Act
        string formatted = CardMasker.Format(clean, [4, 4, 4, 4]);

        // Assert
        formatted.Should().Be("1234 5678");
    }
}
