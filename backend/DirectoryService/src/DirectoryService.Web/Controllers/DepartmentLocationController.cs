using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Relationships;
using DirectoryService.Core.Relationships.Features.CreateDepartmentLocation;
using DirectoryService.Core.Relationships.Features.DeleteDepartmentLocation;
using DirectoryService.Web.EndpointResults;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("")]
public class DepartmentLocationController : ControllerBase
{
    [HttpPost("department/{departmentId:guid}/location/{locationId:guid}")]
    public async Task<EndpointResult<Guid>> Create(
        ICommandHandler<Guid, CreateDepartmentLocationCommand> handler,
        Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken,
        bool isPrimary = false)
    {
        var command = new CreateDepartmentLocationCommand(departmentId, locationId, isPrimary);
        return await handler.Handle(
            command,
            cancellationToken);
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
        [FromServices] ICommandHandler<Guid, DeleteDepartmentLocationCommand> handler,
        [FromRoute] Guid departmentId,
        Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDepartmentLocationCommand(departmentId, locationId);
        return await handler.Handle(command, cancellationToken);
    }
}