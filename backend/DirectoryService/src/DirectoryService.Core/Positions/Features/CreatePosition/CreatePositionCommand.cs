using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Positions.Features.CreatePosition;

public record CreatePositionCommand(CreatePositionRequest Request) : ICommand;