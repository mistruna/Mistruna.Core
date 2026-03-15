using System.Linq.Expressions;

namespace Mistruna.Core.Contracts.Base.Infrastructure;

/// <summary>
/// Represents a specification pattern for defining query criteria.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <remarks>
/// The Specification pattern encapsulates query logic that can be combined and reused.
/// </remarks>
/// <example>
/// <code>
/// public class ActiveUsersSpec : Specification&lt;User&gt;
/// {
///     public override Expression&lt;Func&lt;User, bool&gt;&gt; ToExpression()
///         => user => user.IsActive &amp;&amp; !user.IsDeleted;
/// }
///
/// var activeUsers = await repository.FindAsync(new ActiveUsersSpec());
/// </code>
/// </example>
public interface ISpecification<T> where T : class
{
    /// <summary>
    /// Gets the criteria expression for filtering.
    /// </summary>
    Expression<Func<T, bool>>? Criteria { get; }

    /// <summary>
    /// Gets the include expressions for eager loading.
    /// </summary>
    List<Expression<Func<T, object>>> Includes { get; }

    /// <summary>
    /// Gets the string-based include expressions for eager loading.
    /// </summary>
    List<string> IncludeStrings { get; }

    /// <summary>
    /// Gets the order by expression.
    /// </summary>
    Expression<Func<T, object>>? OrderBy { get; }

    /// <summary>
    /// Gets the order by descending expression.
    /// </summary>
    Expression<Func<T, object>>? OrderByDescending { get; }

    /// <summary>
    /// Gets the number of items to take.
    /// </summary>
    int? Take { get; }

    /// <summary>
    /// Gets the number of items to skip.
    /// </summary>
    int? Skip { get; }

    /// <summary>
    /// Gets a value indicating whether paging is enabled.
    /// </summary>
    bool IsPagingEnabled { get; }

    /// <summary>
    /// Gets a value indicating whether to use split queries.
    /// </summary>
    bool IsSplitQuery { get; }

    /// <summary>
    /// Gets a value indicating whether to use no tracking.
    /// </summary>
    bool IsNoTracking { get; }
}

/// <summary>
/// Base implementation of the specification pattern.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public abstract class Specification<T> : ISpecification<T> where T : class
{
    /// <inheritdoc />
    public Expression<Func<T, bool>>? Criteria { get; private set; }

    /// <inheritdoc />
    public List<Expression<Func<T, object>>> Includes { get; } = new();

    /// <inheritdoc />
    public List<string> IncludeStrings { get; } = new();

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderBy { get; private set; }

    /// <inheritdoc />
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }

    /// <inheritdoc />
    public int? Take { get; private set; }

    /// <inheritdoc />
    public int? Skip { get; private set; }

    /// <inheritdoc />
    public bool IsPagingEnabled { get; private set; }

    /// <inheritdoc />
    public bool IsSplitQuery { get; private set; }

    /// <inheritdoc />
    public bool IsNoTracking { get; private set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class.
    /// </summary>
    protected Specification()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Specification{T}"/> class with criteria.
    /// </summary>
    /// <param name="criteria">The criteria expression.</param>
    protected Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds a criteria expression.
    /// </summary>
    /// <param name="criteria">The criteria expression.</param>
    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    /// <summary>
    /// Adds an include expression for eager loading.
    /// </summary>
    /// <param name="includeExpression">The include expression.</param>
    protected void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Adds a string-based include for eager loading.
    /// </summary>
    /// <param name="includeString">The navigation property path.</param>
    protected void AddInclude(string includeString)
    {
        IncludeStrings.Add(includeString);
    }

    /// <summary>
    /// Applies ascending ordering.
    /// </summary>
    /// <param name="orderByExpression">The order by expression.</param>
    protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    /// <summary>
    /// Applies descending ordering.
    /// </summary>
    /// <param name="orderByDescendingExpression">The order by descending expression.</param>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
    {
        OrderByDescending = orderByDescendingExpression;
    }

    /// <summary>
    /// Applies paging.
    /// </summary>
    /// <param name="skip">The number of items to skip.</param>
    /// <param name="take">The number of items to take.</param>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }

    /// <summary>
    /// Enables split queries for better performance with large includes.
    /// </summary>
    protected void EnableSplitQuery()
    {
        IsSplitQuery = true;
    }

    /// <summary>
    /// Enables change tracking.
    /// </summary>
    protected void EnableTracking()
    {
        IsNoTracking = false;
    }
}
