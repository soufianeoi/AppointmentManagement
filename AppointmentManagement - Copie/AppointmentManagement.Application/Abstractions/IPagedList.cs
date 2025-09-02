namespace AppointmentManagement.Application.Abstractions;

public interface IPagedList<T>
{
    IReadOnlyList<T> Items { get; }
    int Page { get; }
    int PageSize { get; }
    int TotalCount { get; }
    bool HasNextPage { get; }
    bool HasPreviousPage { get; }
}