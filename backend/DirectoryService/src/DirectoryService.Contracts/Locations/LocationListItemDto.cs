namespace DirectoryService.Contracts.Locations;

public record LocationListItemDto(IList<LocationDto> Locations, long TotalCount);