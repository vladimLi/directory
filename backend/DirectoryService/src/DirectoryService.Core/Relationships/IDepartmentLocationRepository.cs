using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Shared;

namespace DirectoryService.Core.Relationships;

public interface IDepartmentLocationRepository
{
    Task<Result<Guid,Failure>> AddAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
    Task<Result<bool,Failure>> DepartmentExistsAsync(DepartmentId departmentId, CancellationToken cancellationToken);
    Task<Result<bool,Failure>> LocationExistsAsync(LocationId locationId, CancellationToken cancellationToken);
    Task<Result<bool,Failure>> ExistsAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
    Task<Result<Guid,Failure>> DeleteAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
}