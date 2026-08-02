using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Shared;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Result<Guid,Failure>> AddAsync(Location location, CancellationToken cancellationToken);
    Task<Result<bool,Failure>> ExistsWithNameAsync(LocationName locationName, CancellationToken cancellationToken);
    Task<Result<Location,Failure>> GetByIdAsync(
        LocationId locationId,
        CancellationToken cancellationToken);

    Task<UnitResult<Failure>> Save(CancellationToken cancellationToken);
 }