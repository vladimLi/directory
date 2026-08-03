using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Locations;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("locations")]
public class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationsService;

    public LocationsController(ILocationsService locationsService)
    {
        _locationsService = locationsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.Create(request, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
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
    public async Task<IActionResult> UpdateLocationName(
        [FromServices] ILocationsService locationsService,
        [FromBody] UpdateLocationNameRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.UpdateLocationName(request, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
    [HttpPatch("address")]
    public async Task<IActionResult> UpdateLocationAddress(
        [FromServices] ILocationsService locationsService,
        [FromBody] UpdateLocationAddressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _locationsService.UpdateLocationAddress(request, cancellationToken);
        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
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