using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentId
{
    public Guid Value { get; }
    
    private DepartmentId(Guid value) => Value = value;

    public static Result<DepartmentId,Errors> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GeneralErrors.ValueIsEmpty("department.id");

        return new DepartmentId(value);
    }
}