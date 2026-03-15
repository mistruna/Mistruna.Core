using FluentAssertions;
using Mistruna.Core.Exceptions;
using Xunit;

namespace Mistruna.Core.Tests;

public class ExceptionTests
{
    [Fact]
    public void NotFoundException_ShouldHaveCorrectErrorCode()
    {
        // Act
        var exception = new NotFoundException("Entity not found");

        // Assert
        exception.ErrorCode.Should().Be("NOT_FOUND");
        exception.Message.Should().Be("Entity not found");
    }

    [Fact]
    public void NotFoundException_WithCustomErrorCode_ShouldUseCustomCode()
    {
        // Act
        var exception = new NotFoundException("User not found", "USER_NOT_FOUND");

        // Assert
        exception.ErrorCode.Should().Be("USER_NOT_FOUND");
    }

    [Fact]
    public void NotFoundException_For_ShouldCreateTypedNotFound()
    {
        // Act
        var exception = NotFoundException.For<TestEntity>(123);

        // Assert
        exception.ErrorCode.Should().Be("TESTENTITY_NOT_FOUND");
        exception.Message.Should().Contain("TestEntity");
        exception.Message.Should().Contain("123");
    }

    [Fact]
    public void ConflictException_ShouldHaveCorrectErrorCode()
    {
        // Act
        var exception = new ConflictException("Duplicate email");

        // Assert
        exception.ErrorCode.Should().Be("CONFLICT");
        exception.Message.Should().Be("Duplicate email");
    }

    [Fact]
    public void ConflictException_For_ShouldCreateTypedConflict()
    {
        // Act
        var exception = ConflictException.For<TestEntity>("test@email.com");

        // Assert
        exception.ErrorCode.Should().Be("TESTENTITY_ALREADY_EXISTS");
        exception.Message.Should().Contain("TestEntity");
    }

    [Fact]
    public void ConflictException_ForField_ShouldCreateFieldConflict()
    {
        // Act
        var exception = ConflictException.ForField<TestEntity>("Email", "test@email.com");

        // Assert
        exception.ErrorCode.Should().Be("TESTENTITY_DUPLICATE_EMAIL");
        exception.Message.Should().Contain("Email");
        exception.Message.Should().Contain("test@email.com");
    }

    [Fact]
    public void ForbiddenAccessException_ShouldHaveCorrectErrorCode()
    {
        // Act
        var exception = new ForbiddenAccessException();

        // Assert
        exception.ErrorCode.Should().Be("FORBIDDEN");
        exception.Message.Should().Be("Access denied");
    }

    [Fact]
    public void ForbiddenAccessException_ForRole_ShouldCreateRoleException()
    {
        // Act
        var exception = ForbiddenAccessException.ForRole("Admin");

        // Assert
        exception.ErrorCode.Should().Be("FORBIDDEN_ROLE");
        exception.Message.Should().Contain("Admin");
    }

    [Fact]
    public void ForbiddenAccessException_ForPermission_ShouldCreatePermissionException()
    {
        // Act
        var exception = ForbiddenAccessException.ForPermission("CanEdit");

        // Assert
        exception.ErrorCode.Should().Be("FORBIDDEN_PERMISSION");
        exception.Message.Should().Contain("CanEdit");
    }

    [Fact]
    public void BadRequestException_ShouldHaveCorrectProperties()
    {
        // Act
        var exception = new BadRequestException("Invalid input", "INVALID_INPUT", new { Field = "Name" });

        // Assert
        exception.ErrorCode.Should().Be("INVALID_INPUT");
        exception.Message.Should().Be("Invalid input");
        exception.Details.Should().NotBeNull();
    }

    private class TestEntity { }
}
