namespace DirectoryService.Domain.Positions.ValueObjects;

public record PositionId
{
    public Guid Value { get; }

    private PositionId(Guid value) => Value = value;

    public static PositionId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value cannot be empty", nameof(value));

        return new(value);
    }
}