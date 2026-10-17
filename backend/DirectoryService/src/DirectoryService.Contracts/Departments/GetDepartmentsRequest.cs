namespace DirectoryService.Contracts.Departments;

public record GetDepartmentsRequest(
    string? Search,
    string? SortBy,
    string? SortDirection,
    PaginationRequest Pagination);