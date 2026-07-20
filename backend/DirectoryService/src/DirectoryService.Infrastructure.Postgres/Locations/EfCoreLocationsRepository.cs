using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Locations.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Locations;

public class EfCoreLocationsRepository: ILocationsRepository
{
    private readonly AppDbContext _context;

    public EfCoreLocationsRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return location.Id.Value;
    }

    public async Task<bool> ExistsWithNameAsync(LocationName locationName, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AnyAsync(x => x.Name == locationName, cancellationToken);
    }

    public async Task<Location?> GetByIdAsync(LocationId locationId, CancellationToken cancellationToken)
    {
        var location =  await _context.Locations
            .SingleOrDefaultAsync(l => l.Id == locationId, cancellationToken);

        if (location == null)
        {
            throw new KeyNotFoundException($"No location with id {locationId} found.");
        }
        return location;
    }

    public async Task Save(CancellationToken cancellationToken)
        =>  await _context.SaveChangesAsync(cancellationToken);
}