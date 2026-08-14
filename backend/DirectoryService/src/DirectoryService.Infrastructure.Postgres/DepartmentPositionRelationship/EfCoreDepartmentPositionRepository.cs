using CSharpFunctionalExtensions;
using DirectoryService.Core.DepartmentPositionRelationship;
using DirectoryService.Core.DepartmentPositionRelationship.Errors;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.DepartmentPositionRelationship;

public class EfCoreDepartmentPositionRepository : IDepartmentPositionRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfCoreDepartmentPositionRepository> _logger;
    
    public EfCoreDepartmentPositionRepository(
        AppDbContext context,
        ILogger<EfCoreDepartmentPositionRepository> logger)
    {
        _context  = context;
        _logger = logger;
    }
    
    public async Task<Result<Guid, Errors>> AddAsync(
        DepartmentPosition departmentPosition,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.DepartmentPosition.AddAsync(departmentPosition, cancellationToken);
            
            return Result.Success<Guid, Errors>(departmentPosition.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при создании связи департамента и должности");

            return Result.Failure<Guid, Errors>(
                Fails.DepartmentPositionError.SaveFailedException(ex.Message));
        }
    }

    public async Task<Result<bool, Errors>> DepartmentExistsAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId, cancellationToken);

        if (!exists)
            return Result.Failure<bool, Errors>(Fails.DepartmentPositionError
                .DepartmentNotFoundException(departmentId.Value));

        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<bool, Errors>> PositionExistsAsync(
        PositionId positionId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Positions
            .AnyAsync(p => p.Id == positionId, cancellationToken);

        if (!exists)
            return Result.Failure<bool, Errors>(Fails.DepartmentPositionError
                .PositionNotFoundException(positionId.Value));

        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<bool, Errors>> ExistsAsync(
        DepartmentId departmentId,
        PositionId positionId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.DepartmentPosition
            .AnyAsync(dp => dp.DepartmentId == departmentId && dp.PositionId == positionId, cancellationToken);
        if (exists)
            return Result.Failure<bool, Errors>(Fails.DepartmentPositionError.DepartmentPositionExistsException());

        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<Guid, Errors>> DeleteAsync(
        DepartmentId departmentId,
        PositionId positionId,
        CancellationToken cancellationToken)
    {
        try
        {
            var departmentPosition = await _context.DepartmentPosition
                .FirstOrDefaultAsync(
                    dp => dp.DepartmentId == departmentId && dp.PositionId == positionId,
                    cancellationToken);

            if (departmentPosition is null)
                return Result.Failure<Guid, Errors>(
                    Fails.DepartmentPositionError.DepartmentPositionNotFoundException());
            
            _context.DepartmentPosition.Remove(departmentPosition);

            return Result.Success<Guid, Errors>(departmentPosition.DepartmentId.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при удалении связи между департаментом и должностью");

            return Result.Failure<Guid, Errors>(
                Fails.DepartmentPositionError.SaveFailedException(ex.Message)
            );
        }
    }
}