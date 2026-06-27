using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Relationships.ValueObjects;

namespace DirectoryService.Domain.Relationships
{
    public sealed class DepartmentPosition
    {
        public DepartmentPositionId Id { get; } = null!;
        public DepartmentId DepartmentId { get; }= null!;
        public PositionId PositionId { get; }= null!;
        public DepartmentPosition(){}
        private DepartmentPosition(Guid id, DepartmentId departmentId, PositionId positionId)
        {
            Id = DepartmentPositionId.Create(id);
            DepartmentId = departmentId;
            PositionId = positionId;
        }
        public static DepartmentPosition Create(DepartmentId departmentId, PositionId positionId)
        {
            if (departmentId.Value == Guid.Empty)
                throw new ArgumentException("Department ID cannot be empty.", nameof(departmentId));
            if (positionId.Value == Guid.Empty)
                throw new ArgumentException("Position ID cannot be empty.", nameof(positionId));

            return new(Guid.CreateVersion7(), departmentId, positionId);
        }
    }
}