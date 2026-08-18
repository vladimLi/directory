using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.Commands.UpdateLocationAddress;

public record UpdateLocationAddressCommand(UpdateLocationAddressRequest Request) : ICommand;