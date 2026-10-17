namespace DirectoryService.Contracts.Departments;

public record DepartmentDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } =  string.Empty;
    public string Path { get; init; } =  string.Empty;
    public DateTime CreatedAt { get; init; }
}