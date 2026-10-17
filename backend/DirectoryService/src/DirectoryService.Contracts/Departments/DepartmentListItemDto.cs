namespace DirectoryService.Contracts.Departments;

public record DepartmentListItemDto(IList<DepartmentDto> Departments, long TotalCount);

