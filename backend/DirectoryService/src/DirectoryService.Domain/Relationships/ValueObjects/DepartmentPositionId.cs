using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Relationships.ValueObjects;

public record DepartmentPositionId
{
    public Guid Value { get; }

    private DepartmentPositionId(Guid value) => Value = value;

    public static Result<DepartmentPositionId,Errors> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GeneralErrors.ValueIsEmpty("department.position.id");

        return new DepartmentPositionId(value);
    }
}