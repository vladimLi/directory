
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

        public static Result<LocationAddress,Failure> Create(string street, string city, string country)
        {
            if (string.IsNullOrWhiteSpace(street))
                return GeneralErrors.VauleIsNullOrEmpty("street");
            if (string.IsNullOrWhiteSpace(city))
                return GeneralErrors.VauleIsNullOrEmpty("city");
            if (string.IsNullOrWhiteSpace(country))
                return GeneralErrors.VauleIsNullOrEmpty("country");
            return new LocationAddress(street.Trim(), city.Trim(), country.Trim());
        }

        public static Result<LocationAddress,Failure> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return GeneralErrors.VauleIsNullOrEmpty("location.address");

            return new LocationAddress(value.Trim());
        }
    }
}