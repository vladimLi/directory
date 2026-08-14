using DirectoryService.Core.Abstractions;
using DirectoryService.Core.DepartmentPositionRelationship.Features.CreateDepartmentPosition;
using DirectoryService.Core.DepartmentPositionRelationship.Features.DeleteDepartmentPosition;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;
[ApiController]
[Route("")]
public class DepartmentPositionController : ControllerBase
{
    [HttpPost("department/{departmentId:guid}/position/{positionId:guid}")]
    public async Task<EndpointResult<Guid>> Create(
        ICommandHandler<Guid, CreateDepartmentPositionCommand> handler,
        Guid departmentId,
        Guid positionId,
        CancellationToken cancellationToken,
        bool isPrimary = false)
    {
        var command = new CreateDepartmentPositionCommand(departmentId, positionId);
        return await handler.Handle(
            command,
            cancellationToken);
    }
    [HttpDelete("department{departmentId:guid}/position/{positionId:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] ICommandHandler<Guid, DeleteDepartmentPositionCommand> handler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDepartmentPositionCommand(departmentId, positionId);
        return await handler.Handle(command, cancellationToken);
    }
}