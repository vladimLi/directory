using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using Shared;

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
    private Department(){}
    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentSlug slug,
        DepartmentPath path,
        DepartmentId? parentId = null)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Path = path;
        ParentId = parentId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<Department,Failure> Create(
        string name,
        string slug,
        DepartmentPath? parentPath = null,
        DepartmentId? parentId = null)
    {
        var departmentId = DepartmentId.Create(Guid.CreateVersion7());
        if (departmentId.IsFailure)
            return departmentId.Error;
        var departmentName = DepartmentName.Create(name);
        if (departmentName.IsFailure)
            return departmentName.Error;
        var departmentSlug = DepartmentSlug.Create(slug);
        if (departmentSlug.IsFailure)
            return departmentSlug.Error;
        var departmentPath = DepartmentPath
            .Create(departmentSlug.Value.Value, parentPath, parentId);
        if (departmentPath.IsFailure)
            return departmentPath.Error;
        
        return new Department(departmentId.Value,
            departmentName.Value,
            departmentSlug.Value, 
            departmentPath.Value,
            parentId);
    }

    public UnitResult<Failure> UpdateName(string name)
    {
        var newName = DepartmentName.Create(name);
        if(newName.IsFailure)
            return newName.Error;
        Name =  newName.Value;
        return UnitResult.Success<Failure>();
    }

    public UnitResult<Failure> UpdateSlug(string slug)
    {
        var newSlug = DepartmentSlug.Create(slug);
        if (newSlug.IsFailure)
            return newSlug.Error;
        Slug = newSlug.Value;
        return UnitResult.Success<Failure>();
    }
}