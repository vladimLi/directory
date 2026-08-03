using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentPath
{
    public string Value { get; }
    private DepartmentPath(string value) => Value = value;

    public static Result<DepartmentPath,Failure> Create(
    string slug,
    DepartmentPath? parentPath = null,
    DepartmentId? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return GeneralErrors.VauleIsNullOrEmpty("slug");

        if (parentPath != null && parentId == null)
            return GeneralErrors.ConditionIsInvalid
                ("Если указан родительский путь, то также должен быть указан родительский идентификатор.",
                    "parentId");

        if (parentPath == null)
            return new DepartmentPath(slug);

        return new DepartmentPath($"{parentPath.Value}/{slug}");
    }
}
