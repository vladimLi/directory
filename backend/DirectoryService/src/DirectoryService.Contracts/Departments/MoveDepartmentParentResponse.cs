namespace DirectoryService.Contracts.Departments;

public sealed record MoveDepartmentParentResponse(
    Guid Id,
    Guid? ParentId,
    string Path,
    int Depth,
    DateTime UpdatedAt);
