using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using Shared;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Result<Guid,Shared.Errors>> Create(CreateLocationRequest request, CancellationToken cancellationToken);
    Task<Result<Guid,Shared.Errors>> UpdateLocationName(UpdateLocationNameRequest request, CancellationToken cancellationToken);
    Task<Result<Guid,Shared.Errors>> UpdateLocationAddress(UpdateLocationAddressRequest request, CancellationToken cancellationToken);
}