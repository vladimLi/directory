namespace DirectoryService.Domain.Relationships.ValueObjects;

public record DepartmentLocationId
{
    public Guid Value { get; }

    private DepartmentLocationId(Guid value) => Value = value;

    public static DepartmentLocationId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value cannot be empty", nameof(value));

        return new(value);
    }
}