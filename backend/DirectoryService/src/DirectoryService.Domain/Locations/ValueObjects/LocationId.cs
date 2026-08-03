using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationId
{
    public Guid Value { get; }

    private LocationId(Guid value) => Value = value;

    public static Result<LocationId,Failure> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GeneralErrors.ValueIsNull("location.id");

        return new LocationId(value);
    }
}