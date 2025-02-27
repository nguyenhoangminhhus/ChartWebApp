namespace ChartWebApp.Models;

public interface IPagedList<T>
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPages { get; }
    int FirstItemOnPage { get; }
    int LastItemOnPage { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
    IEnumerable<T> Subset { get; }
}