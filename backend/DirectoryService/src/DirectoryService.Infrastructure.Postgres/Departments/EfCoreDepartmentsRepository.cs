using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Core.Departments.Errors;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public class EfCoreDepartmentsRepository : IDepartmentsRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<EfCoreDepartmentsRepository> _logger;
    public EfCoreDepartmentsRepository(AppDbContext context,
        ILogger<EfCoreDepartmentsRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Guid, Errors>> AddAsync(
        Department department,
        IReadOnlyCollection<DepartmentLocation> departmentLocations,
        CancellationToken cancellationToken)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.Departments.AddAsync(department, cancellationToken);
            await _context.DepartmentLocation.AddRangeAsync(departmentLocations, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result.Success<Guid, Errors>(department.Id.Value);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при создании департамента");

            return Result.Failure<Guid, Errors>(
                Fails.DepartmentError.SaveFailedException(ex.Message));
        }
    }

    public async Task<Result<Department, Errors>> GetByIdAsync(
        DepartmentId departmentId,
        CancellationToken cancellationToken)
    {
        var department = await _context.Departments
            .SingleOrDefaultAsync(d => d.Id == departmentId, cancellationToken);

        if (department == null)
            return Result.Failure<Department, Errors>(
                Fails.DepartmentError.DepartmentNotFoundException(departmentId.Value));
        
        return Result.Success<Department, Errors>(department);
    }

    public async Task<Result<bool, Errors>> LocationExistsAsync(
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        var count = await _context.Locations
            .CountAsync(l => locationIds.Contains(l.Id), cancellationToken);

        var exists = count == locationIds.Count;

        if (!exists)
            return Result.Failure<bool,Errors>(Fails.DepartmentError.LocationExistsException());
        
        return Result.Success<bool, Errors>(exists);
    }

    public async Task<UnitResult<Errors>> Save(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Errors>();
        }
        catch  (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
        {
            _logger.LogError(ex, "Ошибка при сохранении изменений в БД");

            return UnitResult.Failure(
                Fails.DepartmentError.SaveFailedException(ex.Message)
            );
        }
    }
}