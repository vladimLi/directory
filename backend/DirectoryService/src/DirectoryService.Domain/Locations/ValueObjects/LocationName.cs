using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public sealed record LocationName
{
    public string Value { get; }

    private LocationName(string value) => Value = value;

    public static Result<LocationName,Errors> Create(string value)
    {
        if(string.IsNullOrEmpty(value))
            return GeneralErrors.VauleIsNullOrEmpty("location.name");
        if (value.Length > LengthConstants.Length50)
            return GeneralErrors.ValueLengthIsInvalid("location.name");
        return new LocationName(value);
    }
}