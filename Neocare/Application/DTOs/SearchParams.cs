using System.ComponentModel.DataAnnotations;

namespace Neocare.Application.DTOs
{
    public class SearchParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "date";
        public string SortDirection { get; set; } = "desc";
        public int? MinStressLevel { get; set; }
        public int? MaxStressLevel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class SearchResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}