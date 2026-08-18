using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Positions.Commands.DeletePosition;

public record DeletePositionCommand(Guid PositionId) : ICommand;