namespace DirectoryService.Contracts.Departments;

public record GetDepartmentByIdResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } =  string.Empty;
    public string Path { get; init; } =  string.Empty;
    public Guid? ParentId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}