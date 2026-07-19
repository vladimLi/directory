using DirectoryService.Contracts.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationRequest request, CancellationToken cancellationToken);
    Task<Guid> UpdateLocationName(UpdateLocationNameRequest request, CancellationToken cancellationToken);
    Task<Guid> UpdateLocationAddress(UpdateLocationAddressRequest request, CancellationToken cancellationToken);
}