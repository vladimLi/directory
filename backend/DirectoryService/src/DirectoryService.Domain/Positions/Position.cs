
using DirectoryService.Domain.Positions.ValueObjects;

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
        private Position(Guid id, string name)
        {
            Id = PositionId.Create(id);
            Name = PositionName.Create(name);
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public static Position Create(string name)
        {
            return new(Guid.CreateVersion7(), name);
        }
    }
}