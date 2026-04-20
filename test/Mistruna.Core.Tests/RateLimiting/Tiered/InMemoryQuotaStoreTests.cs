using FluentAssertions;
using Mistruna.Core.RateLimiting.Tiered;
using Xunit;

namespace Mistruna.Core.Tests.RateLimiting.Tiered;

public class InMemoryQuotaStoreTests
{
    [Fact]
    public async Task IncrementAsync_FirstCall_Returns1_AndTtlEqualsWindow()
    {
        var store = new InMemoryQuotaStore();
        var window = TimeSpan.FromSeconds(60);

        var result = await store.IncrementAsync("key1", window, CancellationToken.None);

        result.Count.Should().Be(1);
        result.RetryAfter.Should().BeLessThanOrEqualTo(window);
        result.RetryAfter.Should().BeGreaterThan(TimeSpan.FromSeconds(58));
    }

    [Fact]
    public async Task IncrementAsync_RepeatedCalls_WithinWindow_Accumulate()
    {
        var store = new InMemoryQuotaStore();

        await store.IncrementAsync("key2", TimeSpan.FromSeconds(60), CancellationToken.None);
        await store.IncrementAsync("key2", TimeSpan.FromSeconds(60), CancellationToken.None);
        var result = await store.IncrementAsync("key2", TimeSpan.FromSeconds(60), CancellationToken.None);

        result.Count.Should().Be(3);
    }

    [Fact]
    public async Task IncrementAsync_DifferentKeys_AreIsolated()
    {
        var store = new InMemoryQuotaStore();

        await store.IncrementAsync("a", TimeSpan.FromSeconds(60), CancellationToken.None);
        await store.IncrementAsync("a", TimeSpan.FromSeconds(60), CancellationToken.None);
        var resultB = await store.IncrementAsync("b", TimeSpan.FromSeconds(60), CancellationToken.None);

        resultB.Count.Should().Be(1);
    }
}
