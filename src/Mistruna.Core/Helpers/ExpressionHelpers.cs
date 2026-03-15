using System.Linq.Expressions;

namespace Mistruna.Core.Helpers;

/// <summary>
/// Provides helper methods for working with expression trees.
/// </summary>
public static class ExpressionHelpers
{
    /// <summary>
    /// Creates a lambda expression for sorting entities by a specified property name.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to sort.</typeparam>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <returns>A lambda expression that can be used with LINQ OrderBy methods.</returns>
    /// <example>
    /// <code>
    /// var sortExpression = ExpressionHelpers.GetSortLambda&lt;User&gt;("LastName");
    /// var sortedUsers = users.AsQueryable().OrderBy(sortExpression);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentException">Thrown when the property name does not exist on the entity type.</exception>
    public static Expression<Func<TEntity, object>> GetSortLambda<TEntity>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda<Func<TEntity, object>>(
            Expression.Convert(property, typeof(object)), parameter);

        return lambda;
    }
}
