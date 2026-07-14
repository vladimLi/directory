using DirectoryService.Domain.Departments;
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
        Guid departmentId,
        CancellationToken cancellationToken);

    public Task<bool> LocationExistsAsync(
        LocationId locationId,
        CancellationToken cancellationToken);
}