using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Mistruna.Core.Abstractions.Persistence;
using Mistruna.Core.Caching.Redis;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Mistruna.Core.Tests.Caching.Redis;

public sealed class RedisCacheServiceTests
{
    [Fact]
    public async Task SetAsync_ThenGetAsync_ReturnsRoundtripValue()
    {
        var storage = new Dictionary<string, RedisValue>();
        var database = new Mock<IDatabase>();

        SetupStringSet(database, storage);
        SetupStringGet(database, storage);

        var connection = new Mock<IConnectionMultiplexer>();
        connection.Setup(c => c.GetDatabase(-1, null)).Returns(database.Object);

        var cache = new RedisCacheService(
            connection.Object,
            NullLogger<RedisCacheService>.Instance,
            Options.Create(new CacheOptions { KeyPrefix = "test" }));

        await cache.SetAsync("item", new CachePayload { Id = 42, Name = "alpha" });

        var result = await cache.GetAsync<CachePayload>("item");

        result.Should().NotBeNull();
        result!.Id.Should().Be(42);
        result.Name.Should().Be("alpha");
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueWhenKeyStored()
    {
        var storage = new Dictionary<string, RedisValue> { ["test:exists"] = "\"yes\"" };
        var database = new Mock<IDatabase>();
        database
            .Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) => storage.ContainsKey(key!));

        var connection = new Mock<IConnectionMultiplexer>();
        connection.Setup(c => c.GetDatabase(-1, null)).Returns(database.Object);

        var cache = new RedisCacheService(
            connection.Object,
            NullLogger<RedisCacheService>.Instance,
            Options.Create(new CacheOptions { KeyPrefix = "test" }));

        var exists = await cache.ExistsAsync("exists");

        exists.Should().BeTrue();
    }

    private static void SetupStringSet(Mock<IDatabase> database, Dictionary<string, RedisValue> storage)
    {
        database
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, When, CommandFlags>(
                (key, value, _, _, _) => storage[key!] = value)
            .ReturnsAsync(true);

        database
            .Setup(db => db.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
            .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                (key, value, _, _, _, _) => storage[key!] = value)
            .ReturnsAsync(true);
    }

    private static void SetupStringGet(Mock<IDatabase> database, Dictionary<string, RedisValue> storage)
    {
        database
            .Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync((RedisKey key, CommandFlags _) =>
                storage.TryGetValue(key!, out var value) ? value : RedisValue.Null);
    }

    private sealed class CachePayload
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
