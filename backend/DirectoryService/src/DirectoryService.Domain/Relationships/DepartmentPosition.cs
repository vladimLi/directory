using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments.ValueObjects;
using DirectoryService.Domain.Positions.ValueObjects;
using DirectoryService.Domain.Relationships.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Relationships
{
    public sealed class DepartmentPosition
    {
        public DepartmentPositionId Id { get; } = null!;
        public DepartmentId DepartmentId { get; } = null!;
        public PositionId PositionId { get; } = null!;

        private DepartmentPosition(){}

        private DepartmentPosition(DepartmentPositionId id, DepartmentId departmentId, PositionId positionId)
        {
            Id = id;
            DepartmentId = departmentId;
            PositionId = positionId;
        }

        public static Result<DepartmentPosition, Failure> Create(Guid departmentId, Guid positionId)
        {
            var departmentPositionId = DepartmentPositionId.Create(Guid.CreateVersion7());
            if (departmentPositionId.IsFailure)
                return departmentPositionId.Error;

            var depId = DepartmentId.Create(departmentId);
            if (depId.IsFailure)
                return depId.Error;

            var posId = PositionId.Create(positionId);
            if (posId.IsFailure)
                return posId.Error;


            return new DepartmentPosition(departmentPositionId.Value,
                depId.Value,
                posId.Value);
        }
    }
}