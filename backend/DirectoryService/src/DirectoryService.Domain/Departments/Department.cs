using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    private const int RootDepth = 0;

    public DepartmentId Id { get; } = null!;
    public DepartmentName Name { get; private set; } = null!;
    public DepartmentSlug Slug { get; private set; } = null!;
    public DepartmentPath Path { get; } = null!;
    public DepartmentId? ParentId { get; }
    public int Depth { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }
    //EF Core
    private Department(){}
    private Department(
        DepartmentId id,
        DepartmentName name,
        DepartmentSlug slug,
        DepartmentPath path,
        DepartmentId? parentId = null,
        int depth = RootDepth)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Path = path;
        ParentId = parentId;
        Depth = depth;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<Department,Errors> Create(
        string name,
        string slug,
        Department? parent = null)
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
            .Create(departmentSlug.Value.Value, parent?.Path, parent?.Id);
        if (departmentPath.IsFailure)
            return departmentPath.Error;
        
        return new Department(departmentId.Value,
            departmentName.Value,
            departmentSlug.Value, 
            departmentPath.Value,
            parent?.Id,
            parent is null ? RootDepth : parent.Depth + 1);
    }

    public UnitResult<Errors> UpdateName(string name)
    {
        var newName = DepartmentName.Create(name);
        if(newName.IsFailure)
            return newName.Error;
        Name =  newName.Value;
        return UnitResult.Success<Errors>();
    }

    public UnitResult<Errors> UpdateSlug(string slug)
    {
        var newSlug = DepartmentSlug.Create(slug);
        if (newSlug.IsFailure)
            return newSlug.Error;
        Slug = newSlug.Value;
        return UnitResult.Success<Errors>();
    }

    public UnitResult<Errors> SoftDelete()
    {
        if (IsDeleted)
            return UnitResult.Success<Errors>();
        
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        return UnitResult.Success<Errors>();
    }
}