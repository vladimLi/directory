namespace DirectoryService.Contracts.Locations;

public record LocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int DepartmentCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
