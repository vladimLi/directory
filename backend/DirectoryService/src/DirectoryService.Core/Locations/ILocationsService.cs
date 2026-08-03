using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using Shared;

namespace DirectoryService.Core.Locations;

public interface ILocationsService
{
    Task<Result<Guid,Failure>> Create(CreateLocationRequest request, CancellationToken cancellationToken);
    Task<Result<Guid,Failure>> UpdateLocationName(UpdateLocationNameRequest request, CancellationToken cancellationToken);
    Task<Result<Guid,Failure>> UpdateLocationAddress(UpdateLocationAddressRequest request, CancellationToken cancellationToken);
}