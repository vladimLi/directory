using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Core.Positions;

public interface IPositionRepository
{
    Task<Result<Guid,Shared.Errors>> AddAsync(
        Position  position, 
        CancellationToken cancellationToken);
    Task<Result<bool,Shared.Errors>> ExistsWithNameAsync(
        PositionName positionName,
        CancellationToken cancellationToken);
    Task<Result<Position,Shared.Errors>> GetByIdAsync(
        PositionId positionId,
        CancellationToken cancellationToken);
    Task<Result<Guid, Shared.Errors>> DeleteAsync(
        PositionId positionId,
        CancellationToken cancellationToken);
}