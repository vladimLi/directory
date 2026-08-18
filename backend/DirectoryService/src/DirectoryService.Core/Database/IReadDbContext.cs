using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Core.Database;

public interface IReadDbContext
{
    public IQueryable<Department> DepartmentsRead { get; }
    public IQueryable<Location> LocationsRead { get; }
}