namespace DirectoryService.Domain.Relationships.ValueObjects;

public record DepartmentPositionId
{
    public Guid Value { get; }

    private DepartmentPositionId(Guid value) => Value = value;

    public static DepartmentPositionId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value cannot be empty", nameof(value));

        return new(value);
    }
}