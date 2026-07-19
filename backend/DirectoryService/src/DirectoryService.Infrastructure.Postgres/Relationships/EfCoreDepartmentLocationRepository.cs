using DirectoryService.Core.Relationships;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Relationships;

public class EfCoreDepartmentLocationRepository : IDepartmentLocationRepository
{
    private readonly AppDbContext _context;
    
    public EfCoreDepartmentLocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        await _context.DepartmentLocation.AddAsync(departmentLocation, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return departmentLocation.Id.Value;
    }

    public async Task<bool> DepartmentExistsAsync(DepartmentId departmentId, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .AnyAsync(d => d.Id == departmentId, cancellationToken);
    }

    public async Task<bool> LocationExistsAsync(LocationId locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AnyAsync(l => l.Id == locationId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        return await _context.DepartmentLocation
            .AnyAsync(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId, cancellationToken);
    }

    public async Task<Guid> DeleteAsync(
        DepartmentId departmentId,
        LocationId locationId,
        CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        var departmentLocation = await _context.DepartmentLocation
            .FirstOrDefaultAsync(
                dl => dl.DepartmentId == departmentId && dl.LocationId == locationId,
                cancellationToken);

        if (departmentLocation is null)
            throw new InvalidOperationException("No connection between the department and the location was found");
        
        _context.DepartmentLocation.Remove(departmentLocation);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return departmentLocation.DepartmentId.Value;
    }
}