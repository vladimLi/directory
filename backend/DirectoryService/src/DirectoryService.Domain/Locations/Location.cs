using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    public LocationId Id { get; } = null!;
    public LocationName Name { get; private set; } = null!;
    public LocationAddress Address { get; private set; } = null!;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    //EF Core
    public Location(){}
    private Location(
        Guid id,
        string name,
        string street,
        string city,
        string country)
    {
        Id = LocationId.Create(id);
        Name = LocationName.Create(name);
        Address = LocationAddress.Create(street, city, country);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Location Create(string name, string street, string city, string country)
    {
        return new Location(Guid.CreateVersion7(), name, street, city, country);
    }

    public void UpdateName(string name)
    {
        var newName = LocationName.Create(name);
        Name = newName;
    }
    public void UpdateAddress(
        string street,
        string city,
        string country)
    {
        var newAddress = LocationAddress.Create(street, city, country);
        Address = newAddress;
    }
}