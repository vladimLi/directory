using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;

namespace DirectoryService.Core.Locations.Features.CreateLocation;

public record CreateLocationCommand(CreateLocationRequest Request) : ICommand;