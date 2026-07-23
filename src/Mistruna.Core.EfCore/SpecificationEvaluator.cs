using Microsoft.EntityFrameworkCore;
using Mistruna.Core.Abstractions.Persistence;

namespace Mistruna.Core.EfCore;

internal static class SpecificationEvaluator
{
    public static IQueryable<T> GetQuery<T>(
        IQueryable<T> query,
        ISpecification<T> specification)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(specification);

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        query = specification.Includes.Aggregate(
            query,
            static (current, include) => current.Include(include));

        query = specification.IncludeStrings.Aggregate(
            query,
            static (current, include) => current.Include(include));

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
            query = query.Skip(specification.Skip ?? 0);
            if (specification.Take is not null)
            {
                query = query.Take(specification.Take.Value);
            }
        }

        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        return specification.IsNoTracking ? query.AsNoTracking() : query;
    }
}
