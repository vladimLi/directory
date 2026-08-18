using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using DirectoryService.Core.Database;
using DirectoryService.Core.Departments.Errors;
using DirectoryService.Domain.Departments.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace DirectoryService.Core.Departments.Queries;

public class GetDepartmentByIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetDepartmentByIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<GetDepartmentByIdResponse, Shared.Errors>> Handle(
        GetDepartmentByIdRequest request,
        CancellationToken cancellationToken)
    {
        var departmentId = DepartmentId.Create(request.DepartmentId);
        if (departmentId.IsFailure)
            return departmentId.Error;

        var department = await _readDbContext.DepartmentsRead
            .FirstOrDefaultAsync(d => d.Id == departmentId.Value, cancellationToken);

        if (department is null)
            return Fails.DepartmentError.DepartmentNotFoundException(request.DepartmentId);

        return new GetDepartmentByIdResponse()
        {
            Id = department.Id.Value,
            Name = department.Name.Value,
            Slug = department.Slug.Value,
            Path = department.Path.Value,
            ParentId = department.ParentId == null ? null : department.ParentId.Value,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt
        };
    }
}