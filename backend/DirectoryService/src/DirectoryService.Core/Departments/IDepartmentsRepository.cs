using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Guid> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken);
    
    Task<Department?> GetByIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    public Task<bool> LocationExistsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken);
}