namespace DirectoryService.Contracts.Locations;

public record LocationResponse(Guid Id, string Name, LocationAddressDto Address);