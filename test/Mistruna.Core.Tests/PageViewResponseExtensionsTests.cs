using FluentAssertions;
using Mistruna.Core.Abstractions.Responses;
using Xunit;

namespace Mistruna.Core.Tests;

public sealed class PageViewResponseExtensionsTests
{
    [Fact]
    public void WithPage_ShouldPopulateExplicitPaginationMetadata()
    {
        var elements = new List<TestModel>
        {
            new("first")
        };

        var response = new TestPageResponse().WithPage(
            "Loaded",
            elements,
            total: 12,
            page: 3,
            count: 5);

        response.Message.Should().Be("Loaded");
        response.Page.Should().Be(3);
        response.Count.Should().Be(5);
        response.Total.Should().Be(12);
        response.Elements.Should().BeSameAs(elements);
    }

    [Fact]
    public void WithPage_ShouldUseElementCountForSimpleCollections()
    {
        var elements = new List<TestModel>
        {
            new("first"),
            new("second")
        };

        var response = new TestPageResponse().WithPage("Loaded", elements);

        response.Message.Should().Be("Loaded");
        response.Page.Should().Be(1);
        response.Count.Should().Be(2);
        response.Total.Should().Be(2);
        response.Elements.Should().BeSameAs(elements);
    }

    private sealed class TestPageResponse : PageViewResponse<TestModel>;

    private sealed record TestModel(string Name);
}
