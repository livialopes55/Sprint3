namespace MottuApi.Utils
{
    public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize);

    public static class Pagination
    {
        public static PagedResult<T> ToPagedResult<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var total = query.Count();
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>(items, total, pageNumber, pageSize);
        }
    }

    public record Link(string Rel, string Href, string Method);

    public static class Hateoas
    {
        public static object WithLinks<T>(this T resource, IEnumerable<Link> links) where T : class
            => new { data = resource, links = links };
    }
}