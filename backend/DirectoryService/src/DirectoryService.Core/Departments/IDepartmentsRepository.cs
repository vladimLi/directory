using System.Xml.Linq;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Shared;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Failure>> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken);
    
    Task<Result<Department, Failure>> GetByIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

     Task<Result<bool, Failure>> LocationExistsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken);

     Task<UnitResult<Failure>> Save(CancellationToken cancellationToken);
}