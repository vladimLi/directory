using DirectoryService.Core.Relationships;
using DirectoryService.Web.EndpointResults;
using DirectoryService.Web.Extensions;
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
    public async Task<EndpointResult<Guid>> Create(
        Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken,
        bool isPrimary = false)
    {
        return await _departmentLocationService.Create(
            departmentId,
            locationId,
            cancellationToken,
            isPrimary);
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
    public async Task<EndpointResult<Guid>> Delete(
        [FromRoute] Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        return await _departmentLocationService.Delete(departmentId, locationId, cancellationToken);
    }
}