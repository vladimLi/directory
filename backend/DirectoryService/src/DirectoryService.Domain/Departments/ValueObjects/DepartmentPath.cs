namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentPath
{
    public string Value { get; }
    private DepartmentPath(string value) => Value = value;

    public static DepartmentPath Create(
    string slug,
    DepartmentPath? parentPath = null,
    DepartmentId? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Slug cannot be null or empty.", nameof(slug));

        if (parentPath != null && parentId == null)
            throw new ArgumentException(
                "If parent path is provided, parent ID must also be provided.",
                nameof(parentId));

        if (parentPath == null)
            return new(slug);

        return new($"{parentPath.Value}/{slug}");
    }
}
