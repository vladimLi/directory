using DirectoryService.Contracts.PositionsDto;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var response = new PositionResponse(Guid.NewGuid(), request.Name);
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

        var response = new PositionResponse(id, "Stub position");
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPositionsRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<PositionResponse>());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePositionRequest request,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var response = new PositionResponse(id, request.Name);
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