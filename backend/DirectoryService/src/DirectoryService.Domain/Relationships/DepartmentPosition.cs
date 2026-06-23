
namespace DirectoryService.Domain.Relationships
{
    public sealed class DepartmentPosition
    {
        public Guid Id { get; }
        public Guid DepartmentId { get; }
        public Guid PositionId { get; }
        private DepartmentPosition(Guid departmentId, Guid positionId)
        {
            Id = Guid.CreateVersion7();
            DepartmentId = departmentId;
            PositionId = positionId;
        }
        public static DepartmentPosition Create(Guid departmentId, Guid positionId)
        {
            if (departmentId == Guid.Empty)
                throw new ArgumentException("Department ID cannot be empty.", nameof(departmentId));
            if (positionId == Guid.Empty)
                throw new ArgumentException("Position ID cannot be empty.", nameof(positionId));

            return new(departmentId, positionId);
        }
    }
}