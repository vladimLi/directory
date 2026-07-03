using DirectoryService.Domain.Locations;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);
    
    Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken);
 }