
namespace DirectoryService.Domain.Locations.ValueObjects
{
    public sealed record LocationAddress
    {
        public string Value { get; }
        private LocationAddress(string value) => Value = value;
        public static LocationAddress Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            return new(value);
        }
    }
}