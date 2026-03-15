using FluentAssertions;
using Mistruna.Core.Extensions;
using Xunit;

namespace Mistruna.Core.Tests;

public class CollectionExtensionsTests
{
    [Fact]
    public void IsNullOrEmpty_WithNull_ShouldReturnTrue()
    {
        // Arrange
        IEnumerable<int>? collection = null;

        // Act & Assert
        collection.IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_WithEmptyCollection_ShouldReturnTrue()
    {
        // Arrange
        var collection = Enumerable.Empty<int>();

        // Act & Assert
        collection.IsNullOrEmpty().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_WithItems_ShouldReturnFalse()
    {
        // Arrange
        var collection = new[] { 1, 2, 3 };

        // Act & Assert
        collection.IsNullOrEmpty().Should().BeFalse();
    }

    [Fact]
    public void HasItems_WithNull_ShouldReturnFalse()
    {
        // Arrange
        IEnumerable<int>? collection = null;

        // Act & Assert
        collection.HasItems().Should().BeFalse();
    }

    [Fact]
    public void HasItems_WithItems_ShouldReturnTrue()
    {
        // Arrange
        var collection = new[] { 1, 2, 3 };

        // Act & Assert
        collection.HasItems().Should().BeTrue();
    }

    [Fact]
    public void OrEmpty_WithNull_ShouldReturnEmpty()
    {
        // Arrange
        IEnumerable<int>? collection = null;

        // Act
        var result = collection.OrEmpty();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void OrEmpty_WithItems_ShouldReturnSameItems()
    {
        // Arrange
        var collection = new[] { 1, 2, 3 };

        // Act
        var result = collection.OrEmpty();

        // Assert
        result.Should().BeEquivalentTo(collection);
    }

    [Fact]
    public void ForEach_ShouldExecuteActionOnEachElement()
    {
        // Arrange
        var collection = new[] { 1, 2, 3 };
        var results = new List<int>();

        // Act
        collection.ForEach(x => results.Add(x * 2));

        // Assert
        results.Should().BeEquivalentTo(new[] { 2, 4, 6 });
    }

    [Fact]
    public void Batch_ShouldSplitIntoCorrectBatches()
    {
        // Arrange
        var collection = new[] { 1, 2, 3, 4, 5 };

        // Act
        var batches = collection.Batch(2).ToList();

        // Assert
        batches.Should().HaveCount(3);
        batches[0].Should().BeEquivalentTo(new[] { 1, 2 });
        batches[1].Should().BeEquivalentTo(new[] { 3, 4 });
        batches[2].Should().BeEquivalentTo(new[] { 5 });
    }

    [Fact]
    public void DistinctBy_ShouldRemoveDuplicates()
    {
        // Arrange
        var items = new[]
        {
            new { Id = 1, Name = "A" },
            new { Id = 1, Name = "B" },
            new { Id = 2, Name = "C" }
        };

        // Act
        var result = items.DistinctBy(x => x.Id).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Select(x => x.Id).Should().BeEquivalentTo(new[] { 1, 2 });
    }

    [Fact]
    public void WhereNotNull_ShouldFilterOutNulls()
    {
        // Arrange
        var collection = new string?[] { "a", null, "b", null, "c" };

        // Act
        var result = collection.WhereNotNull().ToList();

        // Assert
        result.Should().BeEquivalentTo(new[] { "a", "b", "c" });
    }

    [Fact]
    public void Shuffle_ShouldContainSameElements()
    {
        // Arrange
        var original = new[] { 1, 2, 3, 4, 5 };

        // Act
        var shuffled = original.Shuffle().ToList();

        // Assert
        shuffled.Should().BeEquivalentTo(original);
    }
}

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
