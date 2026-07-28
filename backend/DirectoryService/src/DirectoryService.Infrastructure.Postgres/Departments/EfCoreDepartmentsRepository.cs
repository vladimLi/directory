using DirectoryService.Core.Departments;
using DirectoryService.Core.Departments.Errors.Exceptions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
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

    public async Task<Department?> GetByIdAsync(
        DepartmentId departmentId, 
        CancellationToken cancellationToken)
    {
        var department =  await _context.Departments
            .SingleOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

        if (department == null)
            throw new DepartmentNotFoundException(departmentId.Value);
        
        return department;
    }

    public async Task<bool> LocationExistsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        return await _context.Locations
            .CountAsync(l => locationIds.Contains(l.Id), cancellationToken) == locationIds.Count;
    }

    public async Task Save(CancellationToken cancellationToken)
        =>  await _context.SaveChangesAsync(cancellationToken);
}