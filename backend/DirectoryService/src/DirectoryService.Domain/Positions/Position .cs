
using DirectoryService.Domain.Positions.ValueObjects;

namespace DirectoryService.Domain.Positions
{
    public sealed class Position
    {
        public Guid Id { get; }
        public PositionName Name { get; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
        private Position(string name)
        {
            Id = Guid.CreateVersion7();
            Name = PositionName.Create(name);
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
        public static Position Create(string name)
        {
            return new(name);
        }
    }
}