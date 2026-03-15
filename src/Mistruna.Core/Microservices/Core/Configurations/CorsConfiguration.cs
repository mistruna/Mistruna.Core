namespace Mistruna.Core.Microservices.Core.Configurations;

/// <summary>
/// CORS policy configuration options.
/// This class defines the settings for configuring Cross-Origin Resource Sharing (CORS) policies in ASP.NET Core applications.
/// </summary>
public class CorsPolicyOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether any origin is allowed.
    /// When set to true, the CORS policy allows requests from any origin.
    /// When set to false, only origins specified in <see cref="AllowedOrigins"/> are permitted.
    /// </summary>
    /// <remarks>
    /// Default value is true for development convenience, but should be set to false in production with specific allowed origins.
    /// </remarks>
    public bool AllowAnyOrigin { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether any HTTP method is allowed.
    /// When set to true, the CORS policy allows all HTTP methods (GET, POST, PUT, DELETE, etc.).
    /// When set to false, only methods specified in <see cref="AllowedMethods"/> are permitted.
    /// </summary>
    /// <remarks>
    /// Default value is true. For security, consider restricting to specific methods in production.
    /// </remarks>
    public bool AllowAnyMethod { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether any header is allowed.
    /// When set to true, the CORS policy allows all request headers.
    /// When set to false, only headers specified in <see cref="AllowedHeaders"/> are permitted.
    /// </summary>
    /// <remarks>
    /// Default value is true. Common practice is to allow specific headers like "Content-Type", "Authorization", etc.
    /// </remarks>
    public bool AllowAnyHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether credentials (cookies, authorization headers) are allowed.
    /// When set to true, the CORS policy allows credentials to be included in cross-origin requests.
    /// </summary>
    /// <remarks>
    /// This should only be enabled when necessary, as it reduces security. When enabled, <see cref="AllowAnyOrigin"/> should be false.
    /// </remarks>
    public bool AllowCredentials { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed origins.
    /// This property is used when <see cref="AllowAnyOrigin"/> is false.
    /// </summary>
    /// <example>
    /// <code>
    /// AllowedOrigins = new List&lt;string&gt; { "https://example.com", "https://api.example.com" }
    /// </code>
    /// </example>
    public List<string>? AllowedOrigins { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed HTTP methods.
    /// This property is used when <see cref="AllowAnyMethod"/> is false.
    /// </summary>
    /// <example>
    /// <code>
    /// AllowedMethods = new List&lt;string&gt; { "GET", "POST", "PUT" }
    /// </code>
    /// </example>
    public List<string>? AllowedMethods { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed headers.
    /// This property is used when <see cref="AllowAnyHeader"/> is false.
    /// </summary>
    /// <example>
    /// <code>
    /// AllowedHeaders = new List&lt;string&gt; { "Content-Type", "Authorization", "X-Custom-Header" }
    /// </code>
    /// </example>
    public List<string>? AllowedHeaders { get; set; }
}
