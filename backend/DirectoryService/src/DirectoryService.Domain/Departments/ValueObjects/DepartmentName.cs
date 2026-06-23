namespace DirectoryService.Domain.Departments.ValueObjects;

public sealed record DepartmentName
{
    public string Value { get; }
    private DepartmentName(string value) => Value = value;

    public static DepartmentName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        return new(value);
    }
}