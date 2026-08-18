using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.Commands.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand;