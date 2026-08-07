using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentName
{
    public string Value { get; }
    private DepartmentName(string value) => Value = value;

    public static Result<DepartmentName,Errors> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return GeneralErrors.ValueIsNullOrEmpty("department.name");

        if (value.Length > LengthConstants.Length50)
            return GeneralErrors.ValueLengthIsInvalid("department.name");
        
        return new DepartmentName(value);
    }
}