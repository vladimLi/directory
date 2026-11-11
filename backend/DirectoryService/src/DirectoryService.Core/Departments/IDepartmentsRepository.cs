using System.Linq.Expressions;
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
    Task<Result<Guid, Shared.Errors>> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken);
    
    Task<Result<Department, Shared.Errors>> GetByIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);

    Task<Department?> GetByIdIncludingDeletedAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);
    
    Task<Result<Department, Shared.Errors>> GetByAsync(
        Expression<Func<Department, bool>> predicate,
        CancellationToken cancellationToken);

     Task<Result<bool, Shared.Errors>> LocationExistsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken);

     Task<Result<Guid, Shared.Errors>> DeleteAsync(
         DepartmentId departmentId,
         CancellationToken cancellationToken);

     Task<UnitResult<Shared.Errors>> MoveSubtreeAsync(
         Guid departmentId,
         Guid? newParentId,
         string oldPathLtree,
         string newPathLtree,
         int oldDepth,
         int newDepth,
         CancellationToken cancellationToken);
}