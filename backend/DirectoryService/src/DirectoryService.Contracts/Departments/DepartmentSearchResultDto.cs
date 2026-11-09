namespace DirectoryService.Contracts.Departments;

public sealed record DepartmentSearchResultDto
{
    public DepartmentTreeNodeDto Department { get; init; } = null!;
    public IReadOnlyList<DepartmentAncestorDto> Ancestors { get; init; } = [];
}
