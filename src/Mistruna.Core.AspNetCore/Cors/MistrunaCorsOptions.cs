namespace Mistruna.Core.AspNetCore.Cors;

public sealed class MistrunaCorsOptions
{
    public bool AllowAnyOrigin { get; set; } = true;

    public bool AllowAnyMethod { get; set; } = true;

    public bool AllowAnyHeader { get; set; } = true;

    public bool AllowCredentials { get; set; }

    public IList<string> AllowedOrigins { get; } = [];

    public IList<string> AllowedMethods { get; } = [];

    public IList<string> AllowedHeaders { get; } = [];
}
