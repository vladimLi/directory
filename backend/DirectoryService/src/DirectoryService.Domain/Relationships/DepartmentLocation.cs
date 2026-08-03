using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Locations.ValueObjects;
using DirectoryService.Domain.Relationships.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Relationships
{
    public sealed class DepartmentLocation
    {
        public DepartmentLocationId Id { get; } = null!;
        public DepartmentId DepartmentId { get; } = null!;
        public LocationId LocationId { get; } = null!;

        public bool IsPrimary { get; }

        //EF Core
        private DepartmentLocation() { }

        private DepartmentLocation(DepartmentLocationId id,
            DepartmentId departmentId,
            LocationId locationId,
            bool isPrimary)
        {
            Id = id;
            DepartmentId = departmentId;
            LocationId = locationId;
            IsPrimary = isPrimary;
        }

        public static Result<DepartmentLocation, Failure> Create(Guid departmentId, Guid locId,
            bool isPrimary = false)
        {
            var departmentLocationId = DepartmentLocationId.Create(Guid.CreateVersion7());
            if (departmentLocationId.IsFailure)
                return departmentLocationId.Error;
            
            var depId = DepartmentId.Create(departmentId);
            if (depId.IsFailure)
                return depId.Error;
            
            var locationId = LocationId.Create(locId);
            if (locationId.IsFailure)
                return locationId.Error;
            
            return new DepartmentLocation(departmentLocationId.Value,
                depId.Value,
                locationId.Value,
                isPrimary);
        }
    }
}