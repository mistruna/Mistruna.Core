using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mistruna.Core.AspNetCore.Authentication;
using Xunit;

namespace Mistruna.Core.Tests.Authentication.Jwt;

public sealed class JwtAuthorizationRegistrationTests
{
    [Fact]
    public async Task AddMistrunaJwtAuthorization_RegistersBearerSchemeFromConfiguration()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "ConfiguredIssuer",
            ["Jwt:Audience"] = "ConfiguredAudience",
            ["Jwt:Key"] = "12345678901234567890123456789012",
            ["Jwt:KeyId"] = "configured-key-id"
        });

        services.AddLogging();
        services.AddOptions();
        services.AddMistrunaJwtAuthorization(configuration);

        await using var provider = services.BuildServiceProvider();
        var schemeProvider = provider.GetRequiredService<IAuthenticationSchemeProvider>();
        var bearerScheme = await schemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
        var options = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        bearerScheme.Should().NotBeNull();
        options.TokenValidationParameters.ValidIssuer.Should().Be("ConfiguredIssuer");
        options.TokenValidationParameters.ValidAudience.Should().Be("ConfiguredAudience");
        options.TokenValidationParameters.RoleClaimType.Should().Be("role");
        options.TokenValidationParameters.IssuerSigningKey.KeyId.Should().Be("configured-key-id");
    }

    [Fact]
    public void AddMistrunaJwtAuthorization_ThrowsWhenSigningKeyIsMissing()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "ConfiguredIssuer",
            ["Jwt:Audience"] = "ConfiguredAudience"
        });

        var act = () => services.AddMistrunaJwtAuthorization(configuration);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JWT signing key is not configured*");
    }

    [Fact]
    public void AddMistrunaJwtAuthorization_RegistersAdminPolicy()
    {
        var services = new ServiceCollection();
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "12345678901234567890123456789012"
        });

        services.AddMistrunaJwtAuthorization(configuration);

        using var provider = services.BuildServiceProvider();
        var authorizationOptions = provider.GetRequiredService<IOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>>().Value;

        authorizationOptions.GetPolicy(MistrunaAuthorizationPolicies.AdminOnly).Should().NotBeNull();
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
}
