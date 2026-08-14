using CSharpFunctionalExtensions;
using DirectoryService.Core.Positions;
using DirectoryService.Core.Positions.Errors;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Positions.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Positions;

public class EfCorePositionsRepository : IPositionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfCorePositionsRepository> _logger;

    public EfCorePositionsRepository(AppDbContext context,
        ILogger<EfCorePositionsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Errors>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Positions.AddAsync(position, cancellationToken);
        
            return Result.Success<Guid, Errors>(position.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при создании должности");

            return Result.Failure<Guid, Errors>(
                Fails.PositionsError.SaveFailedException(ex.Message));
        }
    }

    public async Task<Result<bool, Errors>> ExistsWithNameAsync(PositionName positionName, CancellationToken cancellationToken)
    {
        bool exists =  await _context.Positions
            .AnyAsync(p => p.Name == positionName, cancellationToken);

        if (exists)
            return Result.Failure<bool, Errors>(
                Fails.PositionsError.PositionNameDuplicateException()
            );
        
        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<Position, Errors>> GetByIdAsync(PositionId positionId, CancellationToken cancellationToken)
    {
        var position =  await _context.Positions
            .SingleOrDefaultAsync(p => p.Id == positionId, cancellationToken);
        if (position == null)
            return Result.Failure<Position, Errors>(
                Fails.PositionsError.PositionNotFoundException(positionId.Value));
        
        return Result.Success<Position, Errors>(position);
    }

    public async Task<Result<Guid, Errors>> DeleteAsync(PositionId positionId, CancellationToken cancellationToken)
    {
        try
        {
            var position = await _context.Positions
                .FirstOrDefaultAsync(
                    p => p.Id == positionId,
                    cancellationToken);

            if (position is null)
                return Result.Failure<Guid, Errors>(
                    Fails.PositionsError.PositionNotFoundException(positionId.Value));
            
            _context.Positions.Remove(position);

            return Result.Success<Guid, Errors>(position.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при удалении должности");

            return Result.Failure<Guid, Errors>(
                Fails.PositionsError.SaveFailedException(ex.Message)
            );
        }
    }
}