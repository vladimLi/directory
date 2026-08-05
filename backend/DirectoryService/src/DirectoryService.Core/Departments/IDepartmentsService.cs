using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Departments;
using Shared;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Result<Guid, Shared.Errors>> Create(CreateDepartmentRequest request, CancellationToken cancellationToken);
    Task<Result<Guid, Shared.Errors>> UpdateDepartmentName(UpdateDepartmentNameRequest request, CancellationToken cancellationToken);
    Task<Result<Guid, Shared.Errors>> UpdateDepartmentSlug(UpdateDepartmentSlugRequest request, CancellationToken cancellationToken);
}