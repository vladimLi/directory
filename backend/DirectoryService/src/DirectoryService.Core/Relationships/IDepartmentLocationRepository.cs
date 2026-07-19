using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;

namespace DirectoryService.Core.Relationships;

public interface IDepartmentLocationRepository
{
    Task<Guid> AddAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken);
    
    Task<bool> DepartmentExistsAsync(DepartmentId departmentId, CancellationToken cancellationToken);
    Task<bool> LocationExistsAsync(LocationId locationId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
    Task<Guid> DeleteAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken);
}