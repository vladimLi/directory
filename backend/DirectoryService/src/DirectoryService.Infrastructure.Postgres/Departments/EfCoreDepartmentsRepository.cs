using DirectoryService.Core.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public class EfCoreDepartmentsRepository: IDepartmentsRepository
{
    private readonly AppDbContext _context;

    public EfCoreDepartmentsRepository(AppDbContext context)
    {
        _context =  context;
    }
    public async Task<Guid> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        await _context.Departments.AddAsync(department, cancellationToken);
        await _context.DepartmentLocation.AddRangeAsync(departmentLocations, cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return department.Id.Value;
    }

    public async Task<Department?> GetByIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        return await _context.Departments
            .SingleOrDefaultAsync(d => d.Id.Value == departmentId, cancellationToken);
    }

    public async Task<bool> LocationExistsAsync(LocationId locationId, CancellationToken cancellationToken)
    {
        return await _context.Locations
            .AnyAsync(l => l.Id == locationId, cancellationToken);
    }
}