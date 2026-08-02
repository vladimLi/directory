
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Positions.ValueObjects;
using Shared;

namespace DirectoryService.Domain.Positions
{
    public sealed class Position
    {
        public PositionId Id { get; } = null!;
        public PositionName Name { get; } = null!;
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
        //EF Core
        public Position(){}
        private Position(PositionId id, PositionName name)
        {
            Id = id;
            Name = name;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public static Result<Position,Failure> Create(string name)
        {
            var positionId = PositionId.Create(Guid.CreateVersion7());
            if (positionId.IsFailure)
                return positionId.Error;
            
            var positionName =  PositionName.Create(name);
            if (positionName.IsFailure)
                return positionName.Error;
            
            return new Position(positionId.Value, positionName.Value);
        }
    }
}