using FluentAssertions;
using Mistruna.Core.Contracts.Base.Results;
using Xunit;

namespace Mistruna.Core.Tests;

public class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessfulResultWithValue()
    {
        // Arrange
        const string value = "test value";

        // Act
        var result = Result.Success(value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResult()
    {
        // Arrange
        var error = Error.Failure("TEST_ERROR", "Test error message");

        // Act
        var result = Result.Failure(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WithValue_ShouldCreateFailedResultWithError()
    {
        // Arrange
        var error = Error.NotFound("ITEM_NOT_FOUND", "Item not found");

        // Act
        var result = Result.Failure<string>(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Value_OnFailure_ShouldThrowException()
    {
        // Arrange
        var error = Error.Failure("TEST_ERROR", "Test error");
        var result = Result.Failure<string>(error);

        // Act & Assert
        result.Invoking(r => _ = r.Value)
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Map_OnSuccess_ShouldTransformValue()
    {
        // Arrange
        var result = Result.Success(5);

        // Act
        var mapped = result.Map(x => x * 2);

        // Assert
        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(10);
    }

    [Fact]
    public void Map_OnFailure_ShouldReturnFailure()
    {
        // Arrange
        var error = Error.Failure("TEST", "Error");
        var result = Result.Failure<int>(error);

        // Act
        var mapped = result.Map(x => x * 2);

        // Assert
        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().Be(error);
    }

    [Fact]
    public void Bind_OnSuccess_ShouldChainResults()
    {
        // Arrange
        var result = Result.Success(5);

        // Act
        var bound = result.Bind(x => Result.Success(x.ToString()));

        // Assert
        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("5");
    }

    [Fact]
    public void Match_ShouldExecuteCorrectFunction()
    {
        // Arrange
        var successResult = Result.Success(10);
        var failureResult = Result.Failure<int>(Error.Failure("ERR", "Error"));

        // Act
        var successMatch = successResult.Match(
            onSuccess: v => $"Value: {v}",
            onFailure: e => $"Error: {e.Code}");

        var failureMatch = failureResult.Match(
            onSuccess: v => $"Value: {v}",
            onFailure: e => $"Error: {e.Code}");

        // Assert
        successMatch.Should().Be("Value: 10");
        failureMatch.Should().Be("Error: ERR");
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccessResult()
    {
        // Act
        Result<string> result = "test";

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    [Fact]
    public void ImplicitConversion_FromError_ShouldCreateFailureResult()
    {
        // Arrange
        var error = Error.Failure("ERR", "Error");

        // Act
        Result<string> result = error;

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void GetValueOrDefault_OnSuccess_ShouldReturnValue()
    {
        // Arrange
        var result = Result.Success("value");

        // Act
        var value = result.GetValueOrDefault("default");

        // Assert
        value.Should().Be("value");
    }

    [Fact]
    public void GetValueOrDefault_OnFailure_ShouldReturnDefault()
    {
        // Arrange
        var result = Result.Failure<string>(Error.Failure("ERR", "Error"));

        // Act
        var value = result.GetValueOrDefault("default");

        // Assert
        value.Should().Be("default");
    }
}

public class ErrorTests
{
    [Fact]
    public void Error_ShouldHaveCorrectProperties()
    {
        // Act
        var error = new Error("USER_NOT_FOUND", "User was not found", ErrorType.NotFound);

        // Assert
        error.Code.Should().Be("USER_NOT_FOUND");
        error.Description.Should().Be("User was not found");
        error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void Error_FactoryMethods_ShouldCreateCorrectTypes()
    {
        // Act
        var failure = Error.Failure("F1", "Failure");
        var validation = Error.Validation("V1", "Validation");
        var notFound = Error.NotFound("N1", "Not found");
        var conflict = Error.Conflict("C1", "Conflict");
        var unauthorized = Error.Unauthorized("U1", "Unauthorized");
        var forbidden = Error.Forbidden("FB1", "Forbidden");

        // Assert
        failure.Type.Should().Be(ErrorType.Failure);
        validation.Type.Should().Be(ErrorType.Validation);
        notFound.Type.Should().Be(ErrorType.NotFound);
        conflict.Type.Should().Be(ErrorType.Conflict);
        unauthorized.Type.Should().Be(ErrorType.Unauthorized);
        forbidden.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public void Error_ImplicitConversionToString_ShouldReturnCode()
    {
        // Arrange
        var error = Error.Failure("ERROR_CODE", "Description");

        // Act
        string code = error;

        // Assert
        code.Should().Be("ERROR_CODE");
    }
}
