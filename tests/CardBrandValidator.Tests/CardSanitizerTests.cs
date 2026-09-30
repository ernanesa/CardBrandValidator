namespace CardBrandValidator.Tests;

using CardBrandValidator.Core.Services;
using FluentAssertions;

public class CardSanitizerTests
{
    [Theory]
    [InlineData("4532 0151 1283 0366", "4532015112830366")]
    [InlineData("5502-0983-2234-1109", "5502098322341109")]
    [InlineData(" 3782.8224.6310.005 ", "378282246310005")]
    [InlineData("123456789012", "123456789012")]
    public void Sanitize_WithDelimiters_ReturnsOnlyDigits(string input, string expected)
    {
        // Act
        string result = CardSanitizer.Sanitize(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Sanitize_WithNullOrWhitespace_ReturnsEmpty()
    {
        // Act & Assert
        CardSanitizer.Sanitize(null).Should().BeEmpty();
        CardSanitizer.Sanitize(string.Empty).Should().BeEmpty();
        CardSanitizer.Sanitize("   ").Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_WithVeryLongInput_HandlesBufferCorrectly()
    {
        // Arrange (length > 128 to trigger non-stackalloc branch)
        string input = new string('1', 150) + " - " + new string('2', 10);
        string expected = new string('1', 150) + new string('2', 10);

        // Act
        string result = CardSanitizer.Sanitize(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("4532-0151-1283-0366", false)]
    [InlineData("4532 0151 1283 0366", false)]
    [InlineData("4532.0151.1283.0366", false)]
    [InlineData("4532015112830366", false)]
    [InlineData("4532-ABCD-1283", true)]
    [InlineData("4532@0151!1283", true)]
    public void HasInvalidCharacters_EvaluatesCorrectly(string input, bool expected)
    {
        // Act
        bool result = CardSanitizer.HasInvalidCharacters(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void HasInvalidCharacters_WithNullOrEmpty_ReturnsFalse()
    {
        // Act & Assert
        CardSanitizer.HasInvalidCharacters(null).Should().BeFalse();
        CardSanitizer.HasInvalidCharacters(string.Empty).Should().BeFalse();
    }
}
