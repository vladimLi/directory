using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    public Guid Id { get; }
    public LocationName Name { get; }
    public LocationAddress Address { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    private Location(
        string name,
        string address)
    {
        Id = Guid.CreateVersion7();
        Name = LocationName.Create(name);
        Address = LocationAddress.Create(address);
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Location Create(string name, string address)
    {
        return new Location(name, address);
    }
}