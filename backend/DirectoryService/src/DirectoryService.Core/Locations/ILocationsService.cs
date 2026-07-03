using DirectoryService.Contracts.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Guid> Create(CreateLocationRequest request, CancellationToken cancellationToken);
}