// Utils/PagedResult.cs
using System.Text.Json.Serialization;

namespace MottuApi.Utils
{
    public class PagedResult<T>
    {
        public IReadOnlyCollection<T> Data { get; init; } = Array.Empty<T>();
        public int Total { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

       
        [JsonPropertyName("_links")]
        public IEnumerable<Link> Links { get; init; } = Enumerable.Empty<Link>();
    }
}