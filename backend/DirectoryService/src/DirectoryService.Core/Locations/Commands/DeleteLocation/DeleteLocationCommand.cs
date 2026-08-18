using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.Commands.DeleteLocation;

public record DeleteLocationCommand(Guid LocationId) : ICommand;