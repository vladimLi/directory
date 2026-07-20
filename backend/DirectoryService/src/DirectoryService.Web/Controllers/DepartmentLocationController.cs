using DirectoryService.Core.Relationships;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("")]
public class DepartmentLocationController : ControllerBase
{
    private readonly IDepartmentLocationService _departmentLocationService;

    public DepartmentLocationController(IDepartmentLocationService departmentLocationService)
    {
        _departmentLocationService = departmentLocationService;
    }

    [HttpPost("department/{departmentId:guid}/location/{locationId:guid}")]
    public async Task<IActionResult> Create(
        Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken,
        bool isPrimary = false)
    {
        var departmentLocationId = await _departmentLocationService.Create(
            departmentId,
            locationId,
            cancellationToken,
            isPrimary);

        return CreatedAtAction(nameof(GetById), new { id = departmentLocationId }, departmentLocationId);
    }

    [HttpGet("department-location/{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        return Ok(id);
    }

    [HttpDelete("department{departmentId:guid}/location/{locationId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        var departmentLocationId = await _departmentLocationService.Delete(departmentId, locationId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = departmentLocationId }, departmentLocationId);
    }
}