using CSharpFunctionalExtensions;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Locations.Errors;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public class EfCoreLocationsRepository: ILocationsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfCoreLocationsRepository> _logger;
    public EfCoreLocationsRepository(AppDbContext context,
        ILogger<EfCoreLocationsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<Result<Guid, Errors>> AddAsync(Location location, CancellationToken cancellationToken)
    {
        try
        {
            await _context.Locations.AddAsync(location, cancellationToken);
        
            return Result.Success<Guid, Errors>(location.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при создании департамента");

            return Result.Failure<Guid, Errors>(
                Fails.LocationsError.SaveFailedException(ex.Message));
        }
    }

    public async Task<Result<bool, Errors>> ExistsWithNameAsync(LocationName locationName, CancellationToken cancellationToken)
    {
        bool exists =  await _context.Locations
            .AnyAsync(x => x.Name == locationName, cancellationToken);

        if (exists)
            return Result.Failure<bool, Errors>(
                Fails.LocationsError.LocationNameDuplicateException()
            );
        
        return Result.Success<bool, Errors>(exists);
    }

    public async Task<Result<Location, Errors>> GetByIdAsync(LocationId locationId, CancellationToken cancellationToken)
    {
        var location =  await _context.Locations
            .SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken);
        if (location == null)
            return Result.Failure<Location, Errors>(
                Fails.LocationsError.LocationNotFoundException(locationId.Value));
        
        return Result.Success<Location, Errors>(location);
    }

    public async Task<Result<Guid, Errors>> DeleteAsync(LocationId locationId, CancellationToken cancellationToken)
    {
        try
        {
            var location = await _context.Locations
                .FirstOrDefaultAsync(
                    l => l.Id == locationId,
                    cancellationToken);

            if (location is null)
                return Result.Failure<Guid, Errors>(
                    Fails.LocationsError.LocationNotFoundException(locationId.Value));
            
            _context.Locations.Remove(location);

            return Result.Success<Guid, Errors>(location.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при удалении локации");

            return Result.Failure<Guid, Errors>(
                Fails.LocationsError.SaveFailedException(ex.Message)
            );
        }
    }
}