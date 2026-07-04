
namespace DirectoryService.Domain.Locations.ValueObjects
{
    public sealed record LocationAddress
    {
        public string Value { get; }

        private LocationAddress(string street, string city, string country)
        {
            Value = $"{country}, г.{city}, ул.{street}";
        }
        public static LocationAddress Create(string street, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be null or empty.", nameof(street));
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty.", nameof(city));
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be null or empty.", nameof(country));
            return new LocationAddress(street.Trim(), city.Trim(), country.Trim());
        }
    }
}