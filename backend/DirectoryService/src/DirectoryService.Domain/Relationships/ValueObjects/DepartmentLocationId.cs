using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Relationships.ValueObjects;

public record DepartmentLocationId
{
    public Guid Value { get; }

    private DepartmentLocationId(Guid value) => Value = value;

    public static Result<DepartmentLocationId,Errors> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GeneralErrors.ValueIsEmpty("department.location.id");

        return new DepartmentLocationId(value);
    }
}