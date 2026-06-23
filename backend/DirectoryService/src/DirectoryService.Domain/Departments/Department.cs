using DirectoryService.Domain.Departments.ValueObjects;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    public Guid Id { get; }
    public DepartmentName Name { get; }
    public DepartmentSlug Slug { get; }
    public DepartmentPath Path { get; }
    public Guid? ParentId { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    private Department(
        string name,
        string slug,
        DepartmentPath? parentPath = null,
        Guid? parentId = null)
    {
        Id = Guid.CreateVersion7();
        Name = DepartmentName.Create(name);
        Slug = DepartmentSlug.Create(slug);
        Path = DepartmentPath.Create(slug, parentPath, parentId);
        ParentId = parentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    public static Department CreateRoot(string name, string slug)
    {
        return new Department(name, slug, parentPath: null, parentId: null);
    }

    public static Department CreateChild(
        string name,
        string slug,
        DepartmentPath parentPath,
        Guid parentId)
    {
        return new Department(name, slug, parentPath, parentId);
    }
}