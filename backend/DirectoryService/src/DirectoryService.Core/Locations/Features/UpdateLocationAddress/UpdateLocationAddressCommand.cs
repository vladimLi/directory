using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.Features.UpdateLocationAddress;

public record UpdateLocationAddressCommand(UpdateLocationAddressRequest Request) : ICommand;