using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid,Shared.Errors>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> ExistsWithNameAsync(LocationName locationName, CancellationToken cancellationToken);
    Task<Result<Location,Shared.Errors>> GetByIdAsync(
        LocationId locationId,
        CancellationToken cancellationToken);
 }