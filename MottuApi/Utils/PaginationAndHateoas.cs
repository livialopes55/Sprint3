// Utils/PaginationAndHateoas.cs
using Microsoft.EntityFrameworkCore;

namespace MottuApi.Utils
{
    public static class PaginationAndHateoas
    {
        public static async Task<PagedResult<T>> ToPagedAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            Func<int, int, IEnumerable<Link>> buildLinks)
        {
            var total = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Data = data,
                Total = total,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Links = buildLinks(pageNumber, pageSize)
            };
        }
    }
}