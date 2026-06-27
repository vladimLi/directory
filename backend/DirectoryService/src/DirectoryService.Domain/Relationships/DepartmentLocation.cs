using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships.ValueObjects;

namespace DirectoryService.Domain.Relationships
{
    public sealed class DepartmentLocation
    {
        public DepartmentLocationId Id { get; } = null!;
        public DepartmentId DepartmentId { get; } = null!;
        public LocationId LocationId { get; } = null!;

        public bool IsPrimary { get; }
        //EF Core
        public DepartmentLocation(){}
        private DepartmentLocation(Guid id, Guid departmentId, Guid locationId, bool isPrimary)
        {
            Id = DepartmentLocationId.Create(id);
            DepartmentId = DepartmentId.Create(departmentId);
            LocationId = LocationId.Create(locationId);
            IsPrimary = isPrimary;
        }
        public static DepartmentLocation Create(Guid departmentId, Guid locationId, bool isPrimary = false)
        {
            if (departmentId == Guid.Empty)
                throw new ArgumentException("Department ID cannot be empty.", nameof(departmentId));
            if (locationId == Guid.Empty)
                throw new ArgumentException("Location ID cannot be empty.", nameof(locationId));

            return new(Guid.CreateVersion7(),departmentId, locationId, isPrimary);
        }
    }
}