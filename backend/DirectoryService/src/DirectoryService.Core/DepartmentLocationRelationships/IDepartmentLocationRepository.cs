using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;

namespace DirectoryService.Core.DepartmentLocationRelationships;

public interface IDepartmentLocationRepository
{
    Task<Result<Guid,Shared.Errors>> AddAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> DepartmentExistsAsync(DepartmentId departmentId, CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> LocationExistsAsync(LocationId locationId, CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> ExistsAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
    Task<Result<Guid,Shared.Errors>> DeleteAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
}