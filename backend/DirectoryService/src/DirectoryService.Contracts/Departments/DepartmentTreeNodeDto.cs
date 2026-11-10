namespace DirectoryService.Contracts.Departments;

public sealed record DepartmentTreeNodeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public int Depth { get; init; }
    public bool HasChildren { get; init; }
}
