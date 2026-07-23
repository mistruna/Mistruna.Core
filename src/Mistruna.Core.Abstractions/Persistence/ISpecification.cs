using System.Linq.Expressions;

namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Encapsulates reusable query criteria.</summary>
public interface ISpecification<T> where T : class
{
    /// <summary>Gets the filter criteria.</summary>
    Expression<Func<T, bool>>? Criteria { get; }
    /// <summary>Gets expression-based includes.</summary>
    List<Expression<Func<T, object>>> Includes { get; }
    /// <summary>Gets string-based includes.</summary>
    List<string> IncludeStrings { get; }
    /// <summary>Gets ascending ordering.</summary>
    Expression<Func<T, object>>? OrderBy { get; }
    /// <summary>Gets descending ordering.</summary>
    Expression<Func<T, object>>? OrderByDescending { get; }
    /// <summary>Gets the take count.</summary>
    int? Take { get; }
    /// <summary>Gets the skip count.</summary>
    int? Skip { get; }
    /// <summary>Gets whether paging is enabled.</summary>
    bool IsPagingEnabled { get; }
    /// <summary>Gets whether split queries are enabled.</summary>
    bool IsSplitQuery { get; }
    /// <summary>Gets whether change tracking is disabled.</summary>
    bool IsNoTracking { get; }
}

/// <summary>Base implementation of a query specification.</summary>
public abstract class Specification<T> : ISpecification<T> where T : class
{
    /// <summary>Initializes an empty specification.</summary>
    protected Specification() { }
    /// <summary>Initializes a specification with criteria.</summary>
    protected Specification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

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

    /// <summary>Sets filter criteria.</summary>
    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    /// <summary>Adds an expression include.</summary>
    protected void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
    /// <summary>Adds a string include.</summary>
    protected void AddInclude(string includeString) => IncludeStrings.Add(includeString);
    /// <summary>Sets ascending ordering.</summary>
    protected void ApplyOrderBy(Expression<Func<T, object>> expression) => OrderBy = expression;
    /// <summary>Sets descending ordering.</summary>
    protected void ApplyOrderByDescending(Expression<Func<T, object>> expression) => OrderByDescending = expression;
    /// <summary>Applies paging.</summary>
    protected void ApplyPaging(int skip, int take)
    {
        Skip = skip;
        Take = take;
        IsPagingEnabled = true;
    }
    /// <summary>Enables split queries.</summary>
    protected void EnableSplitQuery() => IsSplitQuery = true;
    /// <summary>Enables change tracking.</summary>
    protected void EnableTracking() => IsNoTracking = false;
}
