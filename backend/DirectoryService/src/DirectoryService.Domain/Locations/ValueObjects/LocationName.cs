namespace DirectoryService.Domain.Locations.ValueObjects;

public sealed record LocationName
{
    public string Value { get; }

    private LocationName(string value) => Value = value;

    public static LocationName Create(string value)
    {
        if(string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        return new(value);
    }
}