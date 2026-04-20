using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Authentication.ApiKey;

namespace Mistruna.Core.Idempotency;

/// <summary>
/// Intercepts mutating requests that carry an <c>Idempotency-Key</c> header. On a first call,
/// buffers the downstream response and stores it in <see cref="IIdempotencyStore"/>. On a
/// repeat call with the same key + user, replays the stored response verbatim without invoking
/// downstream handlers.
/// </summary>
/// <remarks>
/// GET/HEAD and requests without the header pass through unchanged. Keys are scoped to
/// <c>{prefix}:{userSubject}:{idempotencyKey}</c> so one user's key cannot shadow another's.
/// </remarks>
public sealed class IdempotencyMiddleware(
    RequestDelegate next,
    IdempotencyOptions options,
    IIdempotencyStore store,
    ILogger<IdempotencyMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!options.Methods.Contains(context.Request.Method)
            || !context.Request.Headers.TryGetValue(options.HeaderName, out var idemKeyValue)
            || string.IsNullOrWhiteSpace(idemKeyValue))
        {
            await next(context);
            return;
        }

        var userId = context.User.FindFirst(ApiKeyClaimTypes.Subject)?.Value
                     ?? context.User.Identity?.Name
                     ?? "anonymous";
        var storeKey = $"{options.KeyPrefix}:{userId}:{idemKeyValue}";

        var cached = await store.GetAsync(storeKey, context.RequestAborted);
        if (cached is not null)
        {
            logger.LogDebug("Idempotent replay for key {Key}", storeKey);
            context.Response.StatusCode = cached.StatusCode;
            if (cached.ContentType is not null) context.Response.ContentType = cached.ContentType;
            await context.Response.Body.WriteAsync(cached.Body, context.RequestAborted);
            return;
        }

        var originalBody = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await next(context);

            buffer.Position = 0;
            var bodyBytes = buffer.ToArray();

            var snapshot = new IdempotentResponse(
                context.Response.StatusCode,
                context.Response.ContentType,
                bodyBytes);

            await store.SetAsync(storeKey, snapshot, options.Ttl, context.RequestAborted);

            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody, context.RequestAborted);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
}
