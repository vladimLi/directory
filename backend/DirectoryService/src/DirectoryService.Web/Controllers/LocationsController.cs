using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Locations.Commands.CreateLocation;
using DirectoryService.Core.Locations.Commands.DeleteLocation;
using DirectoryService.Core.Locations.Commands.UpdateLocationAddress;
using DirectoryService.Core.Locations.Commands.UpdateLocationName;
using DirectoryService.Core.Locations.Queries;
using DirectoryService.Web.EndpointResults;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> handler,
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<GetLocationByIdResponse?>> GetById(
        [FromServices] GetLocationByIdHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new GetLocationByIdRequest(id), cancellationToken);
    }

    [HttpPatch("name")]
    public async Task<EndpointResult<Guid>> UpdateLocationName(
        [FromServices] ICommandHandler<Guid, UpdateLocationNameCommand> handler,
        [FromBody] UpdateLocationNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationNameCommand(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpPatch("address")]
    public async Task<EndpointResult<Guid>> UpdateLocationAddress(
        [FromServices] ICommandHandler<Guid, UpdateLocationAddressCommand> handler,
        [FromBody] UpdateLocationAddressRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationAddressCommand(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetLocationsRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<LocationResponse>());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var response = new LocationResponse(id, request.Name, request.Address);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] ICommandHandler<Guid, DeleteLocationCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLocationCommand(id);
        return await handler.Handle(command, cancellationToken);
    }
}