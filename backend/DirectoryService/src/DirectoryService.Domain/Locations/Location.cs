using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    public LocationId Id { get; } = null!;
    public LocationName Name { get; private set; } = null!;
    public LocationAddress Address { get; private set; } = null!;
    public DateTime CreatedAt { get; }

    public DateTime UpdatedAt { get; }

    //EF Core
    private Location() { }

    private Location(
        LocationId id,
        LocationName name,
        LocationAddress address)
    {
        Id = id;
        Name = name;
        Address = address;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Result<Location, Failure> Create(string name, string street, string city, string country)
    {
        var locationId = LocationId.Create(Guid.CreateVersion7());
        if (locationId.IsFailure)
            return locationId.Error;

        var locationName = LocationName.Create(name);
        if (locationName.IsFailure)
            return locationName.Error;

        var locationAddress = LocationAddress.Create(street, city, country);
        if (locationAddress.IsFailure)
            return locationAddress.Error;

        return new Location(locationId.Value,
            locationName.Value,
            locationAddress.Value);
    }

    public UnitResult<Failure> UpdateName(string name)
    {
        var newName = LocationName.Create(name);
        if (newName.IsFailure)
            return newName;
        Name = newName.Value;
        return UnitResult.Success<Failure>();
    }

    public UnitResult<Failure> UpdateAddress(
        string street,
        string city,
        string country)
    {
        var newAddress = LocationAddress.Create(street, city, country);
        if (newAddress.IsFailure)
            return newAddress;
        Address = newAddress.Value;
        return UnitResult.Success<Failure>();
    }
}