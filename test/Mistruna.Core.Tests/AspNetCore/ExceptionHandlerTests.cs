using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Mistruna.Core.Samples.BasicApi.Features.Ping;
using Xunit;

namespace Mistruna.Core.Tests.AspNetCore;

public sealed class ExceptionHandlerTests
    : IClassFixture<WebApplicationFactory<PingQuery>>
{
    private readonly HttpClient _client;

    public ExceptionHandlerTests(WebApplicationFactory<PingQuery> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task NotFoundException_Returns_404_ProblemDetails_WithErrorCode_AndTraceId()
    {
        var response = await _client.GetAsync("/errors/not-found");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("X.NotFound", json.GetProperty("errorCode").GetString());
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("traceId").GetString()));
    }
}
