using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Departments;
using DirectoryService.Web.EndpointResults;
using DirectoryService.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentsController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }

    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromBody] CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        return await _departmentsService.Create(request, cancellationToken);
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

        var response = new DepartmentResponse(id, "Stub department", "stub-department");
        return Ok(response);
    }

    [HttpPatch("name")]
    public async Task<EndpointResult<Guid>> UpdateDepartmentName(
        [FromServices] IDepartmentsService departmentsService,
        [FromBody] UpdateDepartmentNameRequest request,
        CancellationToken cancellationToken)
    {
        return await _departmentsService.UpdateDepartmentName(request, cancellationToken);
    }

    [HttpPatch("slug")]
    public async Task<EndpointResult<Guid>> UpdateDepartmentSlug(
        [FromServices] IDepartmentsService departmentsService,
        [FromBody] UpdateDepartmentSlugRequest request,
        CancellationToken cancellationToken)
    {
        return await _departmentsService.UpdateDepartmentSlug(request, cancellationToken);
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