using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Locations;
using DirectoryService.Core.Locations.Features.CreateLocation;
using DirectoryService.Core.Locations.Features.UpdateLocationAddress;
using DirectoryService.Core.Locations.Features.UpdateLocationName;
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
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var response = new LocationResponse(id,
            "Stub location",
            new LocationAddressDto("Main Street", "Moscow", "Russia"));
        return Ok(response);
    }

    [HttpPatch("name")]
    public async Task<EndpointResult<Guid>> UpdateLocationName(
        [FromServices]  ICommandHandler<Guid, UpdateLocationNameCommand> handler,
        [FromBody] UpdateLocationNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationNameCommand(request);
        return await handler.Handle(command, cancellationToken);
    }
    [HttpPatch("address")]
    public async Task<EndpointResult<Guid>> UpdateLocationAddress(
        [FromServices]  ICommandHandler<Guid, UpdateLocationAddressCommand> handler,
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
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }
        return NoContent();
    }
}