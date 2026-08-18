using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Departments.Commands;
using DirectoryService.Core.Departments.Commands.CreateDepartment;
using DirectoryService.Core.Departments.Commands.DeleteDepartment;
using DirectoryService.Core.Departments.Commands.UpdateDepartmentSlug;
using DirectoryService.Core.Departments.Queries;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("departments")]
public class DepartmentsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> handler,
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);
        return await handler.Handle(command, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<GetDepartmentByIdResponse>> GetById(
        [FromServices] GetDepartmentByIdHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(new GetDepartmentByIdRequest(id), cancellationToken);
    }

    [HttpPatch("name")]
    public async Task<EndpointResult<Guid>> UpdateDepartmentName(
        [FromServices] ICommandHandler<Guid, UpdateDepartmentNameCommand> handler,
        [FromBody] UpdateDepartmentNameRequest request,
        CancellationToken cancellationToken)
    {
        return await handler.Handle( new UpdateDepartmentNameCommand(request), cancellationToken);
    }

    [HttpPatch("slug")]
    public async Task<EndpointResult<Guid>> UpdateDepartmentSlug(
        [FromServices] ICommandHandler<Guid, UpdateDepartmentSlugCommand> handler,
        [FromBody] UpdateDepartmentSlugRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentSlugCommand(request);
        return await handler.Handle(command, cancellationToken);
    }


    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetDepartmentsRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(Array.Empty<DepartmentResponse>());
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return NotFound();
        }

        var response = new DepartmentResponse(id, request.Name, request.Slug);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] ICommandHandler<Guid, DeleteDepartmentCommand> handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDepartmentCommand(id);
        return await handler.Handle(command, cancellationToken);
    }
}