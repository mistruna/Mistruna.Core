using FluentAssertions;
using Mistruna.Core.Helpers;
using Xunit;

namespace Mistruna.Core.Tests;

public class GuardTests
{
    [Fact]
    public void AgainstNull_WithNullValue_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? value = null;

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNull(value))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AgainstNull_WithNonNullValue_ShouldNotThrow()
    {
        // Arrange
        var value = new object();

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNull(value))
            .Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmpty_WithNullOrEmptyString_ShouldThrowArgumentException(string? value)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNullOrEmpty(value))
            .Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t\n")]
    public void AgainstNullOrWhiteSpace_WithInvalidString_ShouldThrowArgumentException(string? value)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNullOrWhiteSpace(value))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgainstEmptyGuid_WithEmptyGuid_ShouldThrowArgumentException()
    {
        // Arrange
        var guid = Guid.Empty;

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstEmptyGuid(guid))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgainstEmptyGuid_WithValidGuid_ShouldNotThrow()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstEmptyGuid(guid))
            .Should().NotThrow();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AgainstNegative_WithNegativeValue_ShouldThrowArgumentOutOfRangeException(int value)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNegative(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void AgainstNegative_WithNonNegativeValue_ShouldNotThrow(int value)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNegative(value))
            .Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AgainstNegativeOrZero_WithNonPositiveValue_ShouldThrowArgumentOutOfRangeException(int value)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstNegativeOrZero(value))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AgainstOutOfRange_WithValueInRange_ShouldNotThrow()
    {
        // Arrange
        var value = 5;

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstOutOfRange(value, 1, 10))
            .Should().NotThrow();
    }

    [Fact]
    public void AgainstOutOfRange_WithValueOutOfRange_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var value = 15;

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstOutOfRange(value, 1, 10))
            .Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AgainstLengthExceeded_WithValidLength_ShouldNotThrow()
    {
        // Arrange
        var value = "test";

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstLengthExceeded(value, 10))
            .Should().NotThrow();
    }

    [Fact]
    public void AgainstLengthExceeded_WithExceededLength_ShouldThrowArgumentException()
    {
        // Arrange
        var value = "this is a long string";

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstLengthExceeded(value, 5))
            .Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    public void AgainstInvalidEmail_WithValidEmail_ShouldNotThrow(string email)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstInvalidEmail(email))
            .Should().NotThrow();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid")]
    public void AgainstInvalidEmail_WithInvalidEmail_ShouldThrowArgumentException(string email)
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstInvalidEmail(email))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgainstDefault_WithDefaultValue_ShouldThrowArgumentException()
    {
        // Arrange
        var value = default(int);

        // Act & Assert
        FluentActions.Invoking(() => Guard.AgainstDefault(value))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Against_WithTrueCondition_ShouldThrowArgumentException()
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.Against(true, "Custom error message"))
            .Should().Throw<ArgumentException>()
            .WithMessage("Custom error message*");
    }

    [Fact]
    public void Against_WithFalseCondition_ShouldNotThrow()
    {
        // Act & Assert
        FluentActions.Invoking(() => Guard.Against(false, "Custom error message"))
            .Should().NotThrow();
    }
}
