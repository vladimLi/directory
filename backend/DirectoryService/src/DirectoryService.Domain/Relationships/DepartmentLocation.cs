
namespace DirectoryService.Domain.Relationships
{
    public sealed class DepartmentLocation
    {
        public Guid Id { get; }
        public Guid DepartmentId { get; }
        public Guid LocationId { get; }
        public bool IsPrimary { get; }
        private DepartmentLocation(Guid departmentId, Guid locationId, bool isPrimary)
        {
            Id = Guid.CreateVersion7();
            DepartmentId = departmentId;
            LocationId = locationId;
            IsPrimary = isPrimary;
        }
        public static DepartmentLocation Create(Guid departmentId, Guid locationId, bool isPrimary = false)
        {
            if (departmentId == Guid.Empty)
                throw new ArgumentException("Department ID cannot be empty.", nameof(departmentId));
            if (locationId == Guid.Empty)
                throw new ArgumentException("Location ID cannot be empty.", nameof(locationId));

            return new(departmentId, locationId, isPrimary);
        }
    }
}