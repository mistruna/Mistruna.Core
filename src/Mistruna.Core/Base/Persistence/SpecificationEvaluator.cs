using Microsoft.EntityFrameworkCore;
using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Base.Persistence;

/// <summary>
/// Evaluates <see cref="ISpecification{T}"/> against an <see cref="IQueryable{T}"/>,
/// applying criteria, includes, ordering, paging, tracking, and split-query options.
/// </summary>
public static class SpecificationEvaluator
{
    /// <summary>
    /// Applies the specification to the given queryable.
    /// </summary>
    public static IQueryable<T> GetQuery<T>(IQueryable<T> inputQuery, ISpecification<T> specification)
        where T : class
    {
        var query = inputQuery;

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }

        foreach (var includeString in specification.IncludeStrings)
        {
            query = query.Include(includeString);
        }

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.IsPagingEnabled)
        {
            if (specification.Skip.HasValue)
            {
                query = query.Skip(specification.Skip.Value);
            }

            if (specification.Take.HasValue)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        if (specification.IsNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }
}
