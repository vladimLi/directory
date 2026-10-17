namespace DirectoryService.Contracts.Departments;

public record PaginationRequest(int Page = 1, int PageSize = 20);