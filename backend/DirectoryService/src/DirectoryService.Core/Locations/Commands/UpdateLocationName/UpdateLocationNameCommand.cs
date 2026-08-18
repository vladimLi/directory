using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.Commands.UpdateLocationName;

public record UpdateLocationNameCommand(UpdateLocationNameRequest Request) : ICommand;
