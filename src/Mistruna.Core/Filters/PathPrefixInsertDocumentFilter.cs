using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mistruna.Core.Filters;

/// <summary>
/// Swagger document filter that prepends a path prefix to all API routes.
/// Useful when the API is hosted under a sub-path (e.g., "/api/v1").
/// </summary>
public class PathPrefixInsertDocumentFilter(string prefix) : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = swaggerDoc.Paths.Keys.ToList();
        foreach (var path in paths)
        {
            var pathToChange = swaggerDoc.Paths[path];
            swaggerDoc.Paths.Remove(path);
            swaggerDoc.Paths.Add($"{prefix}{path}", pathToChange);
        }
    }
}
