namespace DirectoryService.Domain.Locations.ValueObjects;

public sealed record LocationName
{
    public string Value { get; }

    private LocationName(string value) => Value = value;

    public static LocationName Create(string value)
    {
        if(string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        if (value.Length > LengthConstants.Length50 || value.Length > LengthConstants.Length500)
            throw new ArgumentException("Value is too long.", nameof(value));
        return new(value);
    }
}