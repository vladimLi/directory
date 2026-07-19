using DirectoryService.Contracts.Departments;

namespace DirectoryService.Core.Departments;

public interface IDepartmentsService
{
    Task<Guid> Create(CreateDepartmentRequest request, CancellationToken cancellationToken);
    Task<Guid> UpdateDepartmentName(UpdateDepartmentNameRequest request, CancellationToken cancellationToken);
    Task<Guid> UpdateDepartmentSlug(UpdateDepartmentSlugRequest request, CancellationToken cancellationToken);
}