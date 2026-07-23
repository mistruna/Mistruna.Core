using FluentAssertions;
using Mistruna.Core.Extensions;
using Xunit;

namespace Mistruna.Core.Tests;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("test", "test")]
    public void NullIfEmpty_ShouldReturnCorrectValue(string? input, string? expected)
    {
        // Act & Assert
        input.NullIfEmpty().Should().Be(expected);
    }

    [Theory]
    [InlineData(null, "default", "default")]
    [InlineData("", "default", "default")]
    [InlineData("value", "default", "value")]
    public void DefaultIfEmpty_ShouldReturnCorrectValue(string? input, string defaultValue, string expected)
    {
        // Act & Assert
        input.DefaultIfEmpty(defaultValue).Should().Be(expected);
    }

    [Fact]
    public void Truncate_ShouldTruncateWithEllipsis()
    {
        // Arrange
        var value = "This is a long string";

        // Act
        var result = value.Truncate(10);

        // Assert
        result.Should().Be("This is...");
    }

    [Fact]
    public void Truncate_ShortString_ShouldReturnOriginal()
    {
        // Arrange
        var value = "Short";

        // Act
        var result = value.Truncate(10);

        // Assert
        result.Should().Be("Short");
    }

    [Fact]
    public void ToCamelCase_ShouldConvertCorrectly()
    {
        // Act & Assert
        "TestString".ToCamelCase().Should().Be("testString");
        "Already".ToCamelCase().Should().Be("already");
    }

    [Fact]
    public void ToPascalCase_ShouldConvertCorrectly()
    {
        // Act & Assert
        "testString".ToPascalCase().Should().Be("TestString");
    }

    [Fact]
    public void ToSnakeCase_ShouldConvertCorrectly()
    {
        // Act & Assert
        "TestString".ToSnakeCase().Should().Be("test_string");
        "AlreadyCase".ToSnakeCase().Should().Be("already_case");
    }

    [Fact]
    public void ToKebabCase_ShouldConvertCorrectly()
    {
        // Act & Assert
        "TestString".ToKebabCase().Should().Be("test-string");
    }

    [Fact]
    public void RemoveWhitespace_ShouldRemoveAllWhitespace()
    {
        // Arrange
        var value = "Hello World \t\n Test";

        // Act
        var result = value.RemoveWhitespace();

        // Assert
        result.Should().Be("HelloWorldTest");
    }

    [Fact]
    public void ContainsIgnoreCase_ShouldMatchCaseInsensitive()
    {
        // Act & Assert
        "Hello World".ContainsIgnoreCase("WORLD").Should().BeTrue();
        "Hello World".ContainsIgnoreCase("xyz").Should().BeFalse();
    }

    [Fact]
    public void EqualsIgnoreCase_ShouldMatchCaseInsensitive()
    {
        // Act & Assert
        "Test".EqualsIgnoreCase("TEST").Should().BeTrue();
        "Test".EqualsIgnoreCase("Different").Should().BeFalse();
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name@domain.org", true)]
    [InlineData("invalid", false)]
    [InlineData("@invalid.com", false)]
    [InlineData(null, false)]
    public void IsValidEmail_ShouldValidateCorrectly(string? email, bool expected)
    {
        // Act & Assert
        email.IsValidEmail().Should().Be(expected);
    }

    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://test.org/path", true)]
    [InlineData("ftp://invalid", false)]
    [InlineData("not a url", false)]
    public void IsValidUrl_ShouldValidateCorrectly(string? url, bool expected)
    {
        // Act & Assert
        url.IsValidUrl().Should().Be(expected);
    }

    [Fact]
    public void Mask_ShouldMaskMiddleCharacters()
    {
        // Act & Assert
        "1234567890".Mask(2, 2).Should().Be("12******90");
        "secret@email.com".Mask(3, 4).Should().Be("sec*********.com");
    }

    [Fact]
    public void SplitAndTrim_ShouldSplitAndRemoveEmpty()
    {
        // Arrange
        var value = "a, b,  c , , d";

        // Act
        var result = value.SplitAndTrim(',');

        // Assert
        result.Should().BeEquivalentTo(new[] { "a", "b", "c", "d" });
    }
}
