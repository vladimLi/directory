using DirectoryService.Domain.Departments.ValueObjects;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    public DepartmentId Id { get; } = null!;
    public DepartmentName Name { get; private set; } = null!;
    public DepartmentSlug Slug { get; private set; } = null!;
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

    public static Department Create(
        string name,
        string slug,
        DepartmentPath? parentPath = null,
        DepartmentId? parentId = null)
    {
        return new Department(Guid.CreateVersion7(), name, slug, parentPath, parentId);
    }

    public void UpdateName(string name)
    {
        var newName = DepartmentName.Create(name);
        Name =  newName;
    }
    public void UpdateSlug(string slug)
    {
        var newSlug = DepartmentSlug.Create(slug);
        Slug =  newSlug;
    }
}