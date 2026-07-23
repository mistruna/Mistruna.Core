using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Mistruna.Core.Abstractions.Results;
using Mistruna.Core.Testing.EfCore;
using Mistruna.Core.Testing.Results;
using Xunit;

namespace Mistruna.Core.Tests.Testing;

public sealed class TestAsyncProviderTests
{
    [Fact]
    public async Task AsyncQueryable_ShouldSupportEfCoreAsyncLinq()
    {
        var items = new[] { 1, 2, 3 };
        var queryable = AsyncQueryable.From(items);

        var count = await queryable.CountAsync();
        var list = await queryable.ToListAsync();

        count.Should().Be(3);
        list.Should().Equal(items);
    }

    [Fact]
    public async Task TestAsyncEnumerable_ShouldEnumerateAsynchronously()
    {
        var items = new[] { 10, 20 };
        var values = new List<int>();

        await foreach (var item in new TestAsyncEnumerable<int>(items))
            values.Add(item);

        values.Should().Equal(items);
    }

    [Fact]
    public async Task TestAsyncQueryProvider_ShouldExecuteAsyncQueries()
    {
        var queryable = AsyncQueryable.From(new[] { "alpha", "beta" });

        var first = await queryable.FirstAsync();

        first.Should().Be("alpha");
    }
}

public sealed class ResultAssertionTests
{
    [Fact]
    public void ShouldBeSuccess_ShouldPassForSuccessfulResult()
    {
        var result = Result.Success("ok");

        result.ShouldBeSuccessWithValue("ok");
    }

    [Fact]
    public void ShouldBeFailure_ShouldPassForFailedResult()
    {
        var error = Error.NotFound("ITEM_NOT_FOUND", "Missing");
        var result = Result.Failure<string>(error);

        result.ShouldBeFailure("ITEM_NOT_FOUND");
    }
}

public sealed class TestingPackageTests
{
    [Fact]
    public void TestingPackage_ShouldExposePublicAsyncTestHelpers()
    {
        typeof(TestAsyncQueryProvider<>).IsPublic.Should().BeTrue();
        typeof(TestAsyncEnumerable<>).IsPublic.Should().BeTrue();
        typeof(AsyncQueryable).IsPublic.Should().BeTrue();
        typeof(ResultTestExtensions).IsPublic.Should().BeTrue();
    }

    [Fact]
    public void TestingPackage_ShouldNotDependOnCoreMonolith()
    {
        typeof(AsyncQueryable)
            .Assembly
            .GetReferencedAssemblies()
            .Should()
            .NotContain(reference => reference.Name == "Mistruna.Core");
    }
}
