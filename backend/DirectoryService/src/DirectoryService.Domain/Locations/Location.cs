using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    public LocationId Id { get; } = null!;
    public LocationName Name { get; } = null!;
    public LocationAddress Address { get; } = null!;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    //EF Core
    public Location(){}
    private Location(
        Guid id,
        string name,
        string address)
    {
        Id = LocationId.Create(id);
        Name = LocationName.Create(name);
        Address = LocationAddress.Create(address);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Location Create(string name, string address)
    {
        return new Location(Guid.CreateVersion7(), name, address);
    }
}