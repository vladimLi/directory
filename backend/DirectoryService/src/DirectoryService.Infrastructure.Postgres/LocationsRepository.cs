using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres;

public class LocationsRepository: ILocationsRepository
{
    private readonly AppDbContext _context;

    public LocationsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return location.Id.Value;
    }

    public async Task<bool> ExistsWithNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AnyAsync(x => x.Name.Value == name, cancellationToken);
    }
}
