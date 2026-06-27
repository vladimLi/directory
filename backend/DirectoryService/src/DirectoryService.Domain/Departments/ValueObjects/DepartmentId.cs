namespace DirectoryService.Domain.Departments.ValueObjects;

public record DepartmentId
{
    public Guid Value { get; }
    
    private DepartmentId(Guid value) => Value = value;

    public static DepartmentId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value cannot be empty", nameof(value));

        return new(value);
    }
}