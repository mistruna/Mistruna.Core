using System.Text.Json;

namespace Mistruna.Core.Messaging.RabbitMq.Internal;

internal static class RabbitMessageSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    internal static ReadOnlyMemory<byte> Serialize<T>(T message)
        where T : class
        => JsonSerializer.SerializeToUtf8Bytes(message, Options);

    internal static T Deserialize<T>(ReadOnlyMemory<byte> body)
        where T : class
        => JsonSerializer.Deserialize<T>(body.Span, Options)
            ?? throw new JsonException($"RabbitMQ message body did not contain a {typeof(T).Name}.");
}
