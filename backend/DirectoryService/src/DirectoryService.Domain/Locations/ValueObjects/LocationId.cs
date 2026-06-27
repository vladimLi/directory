namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationId
{
    public Guid Value { get; }

    private LocationId(Guid value) => Value = value;

    public static LocationId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value cannot be empty", nameof(value));

        return new(value);
    }
}