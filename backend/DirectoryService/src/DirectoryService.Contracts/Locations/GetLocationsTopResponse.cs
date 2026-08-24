namespace DirectoryService.Contracts.Locations;

public record GetLocationsTopResponse(
    IReadOnlyCollection<LocationTopDto> Locations);