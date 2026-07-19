using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);
    Task<bool> ExistsWithNameAsync(LocationName locationName, CancellationToken cancellationToken);
    Task<Location?> GetByIdAsync(
        LocationId locationId,
        CancellationToken cancellationToken);

    Task Save(CancellationToken cancellationToken);
 }