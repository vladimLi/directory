
using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects
{
    public sealed record LocationAddress
    {
        public string Value { get; }

        private LocationAddress(string value)
        {
            Value = value;
        }

        private LocationAddress(string street, string city, string country)
            : this($"{country}, г.{city}, ул.{street}") {}

        public static Result<LocationAddress,Errors> Create(string street, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                return GeneralErrors.ValueIsNullOrEmpty("street");
            if (string.IsNullOrWhiteSpace(city))
                return GeneralErrors.ValueIsNullOrEmpty("city");
            if (string.IsNullOrWhiteSpace(country))
                return GeneralErrors.ValueIsNullOrEmpty("country");
            return new LocationAddress(street.Trim(), city.Trim(), country.Trim());
        }

        public static Result<LocationAddress,Errors> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return GeneralErrors.ValueIsNullOrEmpty("location.address");

            return new LocationAddress(value.Trim());
        }
    }
}