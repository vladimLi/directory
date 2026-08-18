using DirectoryService.Contracts.Positions;
using ICommand = DirectoryService.Core.Abstractions.ICommand;

namespace DirectoryService.Core.Positions.Features.UpdatePositionName;

public record UpdatePositionNameCommand(UpdatePositionNameRequest Request): ICommand;