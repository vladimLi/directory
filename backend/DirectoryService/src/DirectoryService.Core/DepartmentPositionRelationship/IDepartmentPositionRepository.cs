using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Relationships;

namespace DirectoryService.Core.DepartmentPositionRelationship;

public interface IDepartmentPositionRepository
{
    Task<Result<Guid,Shared.Errors>> AddAsync(
        DepartmentPosition departmentPosition,
        CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> DepartmentExistsAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> PositionExistsAsync(
        PositionId positionId,
        CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> ExistsAsync(
        DepartmentId departmentId,
        PositionId positionId,
        CancellationToken cancellationToken);
    Task<Result<Guid,Shared.Errors>> DeleteAsync(
        DepartmentId departmentId,
        PositionId positionId,
        CancellationToken cancellationToken);
}