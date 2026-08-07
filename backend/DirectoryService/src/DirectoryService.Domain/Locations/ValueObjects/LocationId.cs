using CSharpFunctionalExtensions;
using Shared;

namespace DirectoryService.Domain.Locations.ValueObjects;

public record LocationId
{
    public Guid Value { get; }

    private LocationId(Guid value) => Value = value;

    public static Result<LocationId,Errors> Create(Guid value)
    {
        if (value == Guid.Empty)
            return GeneralErrors.ValueIsEmpty("location.id");

        return new LocationId(value);
    }
}