using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Relationships;

namespace DirectoryService.Core.Database;

public interface IReadDbContext
{
    public IQueryable<Department> DepartmentsRead { get; }
    public IQueryable<Location> LocationsRead { get; }
    public IQueryable<Position>  PositionsRead { get; }
    public IQueryable<DepartmentLocation>  DepartmentLocationRead { get; }
    public IQueryable<DepartmentPosition>   DepartmentPositionRead { get; }
    
}