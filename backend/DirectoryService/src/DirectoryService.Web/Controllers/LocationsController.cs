using DirectoryService.Contracts.LocationsDto;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("locations")]
public class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var response = new LocationResponse(Guid.NewGuid(), request.Name, request.Address);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
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

        var response = new LocationResponse(id, "Stub location", new LocationAddressDto("Main Street", "Moscow", "Russia"));
        return Ok(response);
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