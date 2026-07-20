namespace DirectoryService.Contracts.Locations;

public record UpdateLocationAddressRequest(Guid Id, LocationAddressDto Address);