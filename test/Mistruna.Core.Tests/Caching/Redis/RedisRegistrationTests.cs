using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Abstractions.Persistence;
using Mistruna.Core.Caching.Redis;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Mistruna.Core.Tests.Caching.Redis;

public sealed class RedisRegistrationTests
{
    [Fact]
    public void AddMistrunaRedis_RegistersCacheService()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddSingleton(Mock.Of<IConnectionMultiplexer>());

        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Redis"] = "localhost:6379"
        });

        services.AddMistrunaRedis(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetService<ICacheService>().Should().BeOfType<RedisCacheService>();
    }

    [Fact]
    public void AddMistrunaRedis_PrefersMistrunaRedisConnectionString()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddOptions();
        services.AddSingleton(Mock.Of<IConnectionMultiplexer>());

        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Mistruna:Redis:ConnectionString"] = "mistruna:6379",
            ["ConnectionStrings:Redis"] = "fallback:6379"
        });

        services.AddMistrunaRedis(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CacheOptions>>().Value;

        options.KeyPrefix.Should().BeEmpty();
    }

    [Fact]
    public void AddMistrunaRedis_ThrowsWhenConnectionStringMissing()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>());

        var act = () => services.AddMistrunaRedis(configuration);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Redis connection string is not configured*");
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
}
