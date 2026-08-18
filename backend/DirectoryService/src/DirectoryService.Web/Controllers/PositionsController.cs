using DirectoryService.Contracts.Positions;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Positions.Commands.CreatePosition;
using DirectoryService.Core.Positions.Commands.DeletePosition;
using DirectoryService.Core.Positions.Commands.UpdatePositionName;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("positions")]
public class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreatePositionCommand> handler,
        [FromBody] CreatePositionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);
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

    [HttpPatch("name")]
    public async Task<EndpointResult<Guid>> UpdatePositionName(
        [FromServices] ICommandHandler<Guid, UpdatePositionNameCommand> handler,
        [FromBody] UpdatePositionNameRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePositionNameCommand(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] ICommandHandler<Guid, DeletePositionCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeletePositionCommand(id);
        return await handler.Handle(command, cancellationToken);
    }
}