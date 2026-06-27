using DirectoryService.Domain.Departments.ValueObjects;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    public DepartmentId Id { get; } = null!;
    public DepartmentName Name { get; } = null!;
    public DepartmentSlug Slug { get; } = null!;
    public DepartmentPath Path { get; } = null!;
    public DepartmentId? ParentId { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    //EF Core
    public Department(){}
    private Department(
        Guid id,
        string name,
        string slug,
        DepartmentPath? parentPath = null,
        DepartmentId? parentId = null)
    {
        Id = DepartmentId.Create(id);
        Name = DepartmentName.Create(name);
        Slug = DepartmentSlug.Create(slug);
        Path = DepartmentPath.Create(slug, parentPath, parentId);
        ParentId = parentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public static Department CreateRoot( string name, string slug)
    {
        return new Department(Guid.CreateVersion7(), name, slug, parentPath: null, parentId: null);
    }

    public static Department CreateChild(
        string name,
        string slug,
        DepartmentPath parentPath,
        DepartmentId parentId)
    {
        return new Department(Guid.CreateVersion7(), name, slug, parentPath, parentId);
    }
}