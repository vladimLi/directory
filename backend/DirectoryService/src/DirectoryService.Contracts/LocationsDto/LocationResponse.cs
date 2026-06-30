namespace DirectoryService.Contracts.LocationsDto;

public record LocationResponse(Guid Id, string Name, LocationAddressDto Address);