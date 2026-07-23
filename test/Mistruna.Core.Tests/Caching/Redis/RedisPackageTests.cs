using FluentAssertions;
using Mistruna.Core.Abstractions.Persistence;
using Mistruna.Core.Caching.Redis;
using Xunit;

namespace Mistruna.Core.Tests.Caching.Redis;

public sealed class RedisPackageTests
{
    [Fact]
    public void RedisCacheService_ShouldImplementAbstractionsContract()
    {
        typeof(ICacheService)
            .IsAssignableFrom(typeof(RedisCacheService))
            .Should()
            .BeTrue();
    }

    [Fact]
    public void CachingRedisPackage_ShouldNotDependOnAspNetCorePackage()
    {
        typeof(RedisCacheService)
            .Assembly
            .GetReferencedAssemblies()
            .Should()
            .NotContain(reference => reference.Name == "Mistruna.Core.AspNetCore");
    }
}
