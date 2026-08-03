using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using Shared;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Result<Guid, Failure>> Create(CreateDepartmentRequest request, CancellationToken cancellationToken);
    Task<Result<Guid, Failure>> UpdateDepartmentName(UpdateDepartmentNameRequest request, CancellationToken cancellationToken);
    Task<Result<Guid, Failure>> UpdateDepartmentSlug(UpdateDepartmentSlugRequest request, CancellationToken cancellationToken);
}