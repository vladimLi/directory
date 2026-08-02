using CSharpFunctionalExtensions;
using DirectoryService.Core.Relationships;
using DirectoryService.Core.Relationships.Errors;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Relationships;

public class EfCoreDepartmentLocationRepository : IDepartmentLocationRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfCoreDepartmentLocationRepository> _logger;

    public EfCoreDepartmentLocationRepository(AppDbContext context,
        ILogger<EfCoreDepartmentLocationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> AddAsync(DepartmentLocation departmentLocation,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.DepartmentLocation.AddAsync(departmentLocation, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success<Guid, Failure>(departmentLocation.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при создании департамента");

            return Result.Failure<Guid, Failure>(
                Fails.DepartmentLocationError.SaveFailedException(ex.Message));
        }
    }

    public async Task<Result<bool, Failure>> DepartmentExistsAsync(DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId, cancellationToken);

        if (!exists)
            return Result.Failure<bool, Failure>(Fails.DepartmentLocationError
                .DepartmentNotFoundException(departmentId.Value));

        return Result.Success<bool, Failure>(exists);
    }

    public async Task<Result<bool, Failure>> LocationExistsAsync(LocationId locationId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Locations
            .AnyAsync(l => l.Id == locationId, cancellationToken);

        if (!exists)
            return Result.Failure<bool, Failure>(Fails.DepartmentLocationError
                .LocationNotFoundException(locationId.Value));

        return Result.Success<bool, Failure>(exists);
    }

    public async Task<Result<bool, Failure>> ExistsAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.DepartmentLocation
            .AnyAsync(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId, cancellationToken);
        if (!exists)
            return Result.Failure<bool, Failure>(Fails.DepartmentLocationError.DepartmentLocationExistsException());

        return Result.Success<bool, Failure>(exists);
    }

    public async Task<Result<Guid, Failure>> DeleteAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var departmentLocation = await _context.DepartmentLocation
                .FirstOrDefaultAsync(
                    dl => dl.DepartmentId == departmentId && dl.LocationId == locationId,
                    cancellationToken);

            if (departmentLocation is null)
                return Result.Failure<Guid, Failure>(
                    Fails.DepartmentLocationError.DepartmentLocationNotFoundException());
            

            _context.DepartmentLocation.Remove(departmentLocation);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success<Guid, Failure>(departmentLocation.DepartmentId.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при удалении связи между департаментом и локацией");

            return Result.Failure<Guid, Failure>(
                Fails.DepartmentLocationError.SaveFailedException(ex.Message)
            );
        }
    }

}