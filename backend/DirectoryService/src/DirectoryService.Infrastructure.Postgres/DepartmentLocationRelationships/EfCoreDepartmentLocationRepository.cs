using CSharpFunctionalExtensions;
using DirectoryService.Core.DepartmentLocationRelationships;
using DirectoryService.Core.DepartmentLocationRelationships.Errors;
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

    public async Task<Result<Guid, Errors>> AddAsync(DepartmentLocation departmentLocation,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.DepartmentLocation.AddAsync(departmentLocation, cancellationToken);
            
            return Result.Success<Guid, Errors>(departmentLocation.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при создании связи департамента и локации");

            return Result.Failure<Guid, Errors>(
                Fails.DepartmentLocationError.SaveFailedException(ex.Message));
        }
    }

    public async Task<Result<bool, Errors>> DepartmentExistsAsync(DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Departments
            .AnyAsync(d => d.Id == departmentId, cancellationToken);

        if (!exists)
            return Result.Failure<bool, Errors>(Fails.DepartmentLocationError
                .DepartmentNotFoundException(departmentId.Value));

        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<bool, Errors>> LocationExistsAsync(LocationId locationId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.Locations
            .AnyAsync(l => l.Id == locationId, cancellationToken);

        if (!exists)
            return Result.Failure<bool, Errors>(Fails.DepartmentLocationError
                .LocationNotFoundException(locationId.Value));

        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<bool, Errors>> ExistsAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        bool exists = await _context.DepartmentLocation
            .AnyAsync(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId, cancellationToken);
        if (exists)
            return Result.Failure<bool, Errors>(Fails.DepartmentLocationError.DepartmentLocationExistsException());

        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<Guid, Errors>> DeleteAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var departmentLocation = await _context.DepartmentLocation
                .FirstOrDefaultAsync(
                    dl => dl.DepartmentId == departmentId && dl.LocationId == locationId,
                    cancellationToken);

            if (departmentLocation is null)
                return Result.Failure<Guid, Errors>(
                    Fails.DepartmentLocationError.DepartmentLocationNotFoundException());
            
            _context.DepartmentLocation.Remove(departmentLocation);

            return Result.Success<Guid, Errors>(departmentLocation.DepartmentId.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при удалении связи между департаментом и локацией");

            return Result.Failure<Guid, Errors>(
                Fails.DepartmentLocationError.SaveFailedException(ex.Message)
            );
        }
    }

}